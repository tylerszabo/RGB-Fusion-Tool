// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;

namespace GLedApiDotNet.Raw
{
    public class GLedAPIv1_0_0Exception : GLedAPIException
    {
        public uint ErrorCode { get; private set; }
        public string ApiFunction { get; private set; }

        private static string FormatCode(string apiFunction, uint errorCode, string message = null)
        {
            switch (errorCode)
            {
                case 0x32:
                    return string.Format("{0} returned ERROR_NOT_SUPPORTED", apiFunction);
                case 0x7A:
                    return string.Format("{0} returned ERROR_INVALID_OPERATION", apiFunction);
                case 0x10DD:
                    return string.Format("{0} returned ERROR_INSUFFICIENT_BUFFER", apiFunction);
                default:
                    return string.Format("{0} returned 0x{1:X}", apiFunction, errorCode);
            }
        }

        public GLedAPIv1_0_0Exception(string apiFunction, uint errorCode, Exception innerException = null)
            : base(FormatCode(apiFunction, errorCode), innerException)
        {
            this.ApiFunction = apiFunction;
            this.ErrorCode = errorCode;
        }
    }
}