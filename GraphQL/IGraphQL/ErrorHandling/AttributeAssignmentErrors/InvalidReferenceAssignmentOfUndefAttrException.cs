﻿using System;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    public sealed class InvalidReferenceAssignmentOfUndefAttrException : AGraphQLException
    {
        /// <summary>
        /// Creates a new InvalidReferenceAssignmentOfUndefAttrException exception
        /// </summary>
        public InvalidReferenceAssignmentOfUndefAttrException()
        { }

        public override string ToString()
        {
            return "An reference assignment for undefined attributes is not allowed.";   
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.InvalidReferenceAssignmentOfUndefAttr; }
        } 
    }
}