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

using System.Collections.Generic;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Index
{
    internal class IndexDefinition: IIndexDefinition
    {
        #region IIndexDefinition Members

        public string Name { get; internal set; }

        public string IndexTypeName { get; internal set; }

        public string Edition { get; internal set; }

        public bool IsUserdefined { get; internal set; }

        public IList<IPropertyDefinition> IndexedProperties { get; internal set; }

        public IVertexType VertexType { get; internal set; }

        public long ID { get; internal set; }

        public bool IsSingle { get; internal set; }

        public bool IsRange { get; internal set; }

        public bool IsVersioned { get; internal set; }

        public IIndexDefinition SourceIndex { get; internal set; }

        #endregion
    }
}
