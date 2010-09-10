/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

/* <id Name="PandoraDB � ABaseFunction" />
 * <copyright file="ABaseFunction.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>This is the base function class. Each function mus derive this class and implement at least:
 *  FunctionName: The name of the function used in the query itself
 *  TypeOfResult: The result type of the evaluated function
 *  SubstringFunc(): The constructor fills the _Parameters dictionary which defines the function parameters
 *  ExecFunc(...): Is the function itself and will containing the logic. You MUST call PandoraResult result = base.ExecFunc(myParams); at the beginning to verify the parameters number and types
 * <summary>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.ObjectManagement;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.Enums;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.TypeManagement.PandoraTypes;
using sones.Lib.ErrorHandling;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;

using sones.GraphDB.Errors;
using sones.GraphFS.Session;
using sones.Lib.Session;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Functions
{

    /// <summary>
    /// A Function parameter containing the parameter name and the type.
    /// This is used by the parameter definition in the function implementation.
    /// </summary>
    public struct ParameterValue
    {
        private String _Name;
        public String Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private ADBBaseObject _DBType;
        public ADBBaseObject DBType
        {
            get { return _DBType; }
            set { _DBType = value; }
        }

        private Boolean _VariableNumOfParams;
        public Boolean VariableNumOfParams
        {
            get { return _VariableNumOfParams; }
            set { _VariableNumOfParams = value; }
        }

        /// <summary>
        /// A single parameter
        /// </summary>
        /// <param name="myParameterName">The name of the parameter. Currently this is not used.</param>
        /// <param name="myParameterDBType">The DBType of the parameter</param>
        public ParameterValue(String myParameterName, ADBBaseObject myParameterDBType)
        {
            _Name = myParameterName;
            _DBType = myParameterDBType;
            _VariableNumOfParams = false;
        }

        /// <summary>
        /// A single parameter
        /// </summary>
        /// <param name="myParameterName">The name of the parameter. Currently this is not used.</param>
        /// <param name="myParameterDBType">The DBType of the parameter</param>
        /// <param name="myVariableNumOfParams">True if this parameter occurs 1 or more time. This have to be the last parameter!</param>
        public ParameterValue(String myParameterName, ADBBaseObject myParameterDBType, Boolean myVariableNumOfParams)
        {
            _Name = myParameterName;
            _DBType = myParameterDBType;
            _VariableNumOfParams = myVariableNumOfParams;
        }
    }

    public abstract class ABaseFunction
    {

        #region Abstract methods

        /// <summary>
        /// A unique function name. This is used in queries to call the function.
        /// </summary>
        public abstract String FunctionName { get; }

        /// <summary>
        /// The ouput of a describe.
        /// </summary>
        /// <returns></returns>
        public abstract String GetDescribeOutput();

        /// <summary>
        /// This will validate the function to a working base.
        /// </summary>
        /// <param name="workingBase">The working base. Might be null for type independent function calls like CURRENTDATE().</param>
        /// <param name="typeManager"></param>
        /// <returns></returns>
        public abstract bool ValidateWorkingBase(TypeAttribute workingBase, DBTypeManager typeManager);

        #endregion

        /// <summary>
        /// The parameters of the function.
        /// </summary>
        protected List<ParameterValue> Parameters;

        /// <summary>
        /// The Calling object. In case of User.Friends it is the edge 'Friends'
        /// </summary>
        public Object CallingObject { get; set; }

        /// <summary>
        /// The calling TypeAttribute. In case of User.Friends it is the attribute 'Friends'
        /// </summary>
        public TypeAttribute CallingAttribute { get; set; }

        /// <summary>
        /// The Calling db Objectstream which contains the attribute. In case of User.Friends it is the user DBObject
        /// </summary>
        public DBObjectStream CallingDBObjectStream { get; set; }

        #region (public) Methods

        public ABaseFunction()
        {
            Parameters = new List<ParameterValue>();
        }

        public List<ParameterValue> GetParameters()
        {
            return Parameters;
        }

        public ParameterValue GetParameter(Int32 elementAt)
        {
            ParameterValue param;
            if (elementAt >= Parameters.Count)
            {
                param = Parameters.Last();
            }
            else
            {
                param = Parameters.ElementAt(elementAt);
            }
            if (param.VariableNumOfParams)
            {
                return new ParameterValue(param.Name, param.DBType.Clone(param.DBType.Value), param.VariableNumOfParams);
            }
            else
            {
                return param;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myParams">The parameters which must match the _Parameters list defined in the constructor.</param>
        /// <returns>The Value of the PandoraResult is of type FuncParameter. The TypeAttribute of FuncParameter will contain NOT NULL if the TypeOfResult is a DBReference or DBList</returns>
        public virtual Exceptional<FuncParameter> ExecFunc(DBContext dbContext, params FuncParameter[] myParams)
        {

            Boolean containsVariableNumOfParams = Parameters.Exists(p => p.VariableNumOfParams);

            #region check number of parameters

            if (Parameters.Count != myParams.Length && (!containsVariableNumOfParams))
            {
                return new Exceptional<FuncParameter>(new Error_FunctionParameterCountMismatch(this, Parameters.Count, myParams.Length));
            }
            else if (containsVariableNumOfParams && myParams.Length == 0)
            {
                return new Exceptional<FuncParameter>(new Error_FunctionParameterCountMismatch(this, Parameters.Count, myParams.Length));
            }

            #endregion

            #region check parameter types

            Int32 definedParamCounter = 0;
            for (Int32 i = 0; i < myParams.Count(); i++)
            {
                if (!Parameters[definedParamCounter].DBType.IsValidValue(((FuncParameter)myParams[i]).Value))
                {
                    return new Exceptional<FuncParameter>(new Error_FunctionParameterTypeMismatch(Parameters[definedParamCounter].DBType.GetType(), myParams[i].GetType()));
                }
                Parameters[definedParamCounter].DBType.SetValue(((FuncParameter)myParams[i]).Value);

                if (!Parameters[definedParamCounter].VariableNumOfParams) definedParamCounter++;
            }

            #endregion

            return new Exceptional<FuncParameter>();

        }

        #endregion

    }
}