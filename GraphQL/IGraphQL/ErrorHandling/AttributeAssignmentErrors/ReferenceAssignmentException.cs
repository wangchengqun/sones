﻿using System;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    public sealed class ReferenceAssignmentException : AGraphQLException
    {
        public String Info { get; private set; }

        /// <summary>
        /// Creates a new ReferenceAssignmentException exception
        /// </summary>
        /// <param name="myInfo">Variable for additional infos</param>
        public ReferenceAssignmentException(String myInfo)
        {
            Info = myInfo;
        }

        public override string ToString()
        {
            return Info;
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.ReferenceAssignment; }
        }
    }
}