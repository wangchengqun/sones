/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using sones.GraphDB.TypeSystem;
using sones.Library.Arithmetics;
using sones.Library.VersionedPluginManager;
using sones.Plugins.SonesGQL.Aggregates.ErrorHandling;
using ISonesGQLFunction.Structure;

namespace sones.Plugins.SonesGQL.Aggregates
{
    /// <summary>
    /// The aggregate Sum
    /// </summary>
    public sealed class SumAggregate : IGQLAggregate
    {
        #region constructor

        /// <summary>
        /// Creates a new sum aggregate
        /// </summary>
        public SumAggregate()
        {
 
        }

        #endregion

        #region IGQLAggregate

        /// <summary>
        /// Calculates the sum
        /// </summary>
        public FuncParameter Aggregate(IEnumerable<IComparable> myValues, IPropertyDefinition myPropertyDefinition)
        {
            var sumType = myPropertyDefinition.BaseType;
            IComparable sum = null;

            try
            {
                foreach (var value in myValues)
                {
                    if (sum == null)
                    {
                        sum = ArithmeticOperations.Add(sumType, 0, value);
                    }
                    else
                    {
                        sum = ArithmeticOperations.Add(sumType, sum, value);
                    }
                }
            }
            catch (ArithmeticException e)
            {
                throw new InvalidArithmeticAggregationException(sumType, this.PluginShortName, e);
            }

            return new FuncParameter(sum, myPropertyDefinition);
        }

        #endregion

        #region IPluginable

        public string PluginName
        {
            get { return "sones.sum"; }
        }

        public string PluginShortName
        {
            get { return "sum"; }
        }

        public string PluginDescription
        {
            get { return "This aggregate will calculate the sum of the given operands. This aggregate is type dependent and will only operate on numbers."; }
        }

        public PluginParameters<Type> SetableParameters
        {
            get { return new PluginParameters<Type>(); }
        }

        public IPluginable InitializePlugin(String myUniqueString, Dictionary<string, object> myParameters = null)
        {
            return new SumAggregate();
        }

        public void Dispose()
        { }

        #endregion
    }
}
