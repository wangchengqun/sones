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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.Exceptions;
using sones.GraphFS.Objects;
using sones.GraphDB.Errors;

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    public class IndexTypeOptNode : AStructureNode, IAstNodeInit
    {

        private String _IndexType;
        public String IndexType
        {
            get { return _IndexType; }
        }

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var dbContext = context.IContext as DBContext;
            if (parseNode.HasChildNodes())
            {
                if (dbContext.DBPluginManager.HasIndex(parseNode.ChildNodes[1].Token.ValueString))
                {
                    _IndexType = parseNode.ChildNodes[1].Token.ValueString;
                }
                else
                {
                    throw new GraphDBException(new Error_IndexTypeDoesNotExist(parseNode.ChildNodes[1].Token.ValueString));
                }
            }
            else
            {
                _IndexType = null;
            }

        }
        
        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion
    }
}