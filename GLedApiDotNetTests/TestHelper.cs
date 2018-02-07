// Copyright (C) 2018 Tyler Szabo
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GLedApiDotNetTests
{
    internal class TestHelper
    {
        public static void AssertLedSettingsEqual(byte[] expected, byte[] actual)
        {
            Assert.AreEqual(16, actual.Length);

            Assert.AreEqual(expected[00], actual[00], "Offset 00, Reserve0");
            Assert.AreEqual(expected[01], actual[01], "Offset 01, LedMode");
            Assert.AreEqual(expected[02], actual[02], "Offset 02, MaxBrightness");
            Assert.AreEqual(expected[03], actual[03], "Offset 03, MinBrightness");
            try
            {
                Assert.AreEqual(expected[04], actual[04], "Offset 04, dwColor, blue");
                Assert.AreEqual(expected[05], actual[05], "Offset 05, dwColor, green");
                Assert.AreEqual(expected[06], actual[06], "Offset 06, dwColor, red");
                Assert.AreEqual(expected[07], actual[07], "Offset 07, dwColor, white");
            }
            catch (AssertFailedException e)
            {
                throw new AssertFailedException(string.Format("WW-RR-GG-BB Expected <{0:x2}-{1:x2}-{2:x2}-{3:x2}> Actual <{4:x2}-{5:x2}-{6:x2}-{7:x2}>",
                    expected[7], expected[6], expected[5], expected[4],
                    actual[7], actual[6], actual[5], actual[4]) , e);
            }
            try
            {
                Assert.AreEqual(expected[08], actual[08], "Offset 08, wTime0 lo");
                Assert.AreEqual(expected[09], actual[09], "Offset 09, wTime0 hi");
            }
            catch (AssertFailedException e)
            {
                throw new AssertFailedException(string.Format("Expected <0x{0:x2},0x{1:x2}={2:D}> Actual <0x{3:x2},0x{4:x2}={5:D}>",
                    expected[8], expected[9], BitConverter.ToUInt16(expected, 8),
                    actual[8], actual[9], BitConverter.ToUInt16(actual, 8)) , e);
            }
            try
            {
                Assert.AreEqual(expected[10], actual[10], "Offset 10, wTime1 lo");
                Assert.AreEqual(expected[11], actual[11], "Offset 11, wTime1 hi");
            }
            catch (AssertFailedException e)
            {
                throw new AssertFailedException(string.Format("Expected <0x{0:x2},0x{1:x2}={2:D}> Actual <0x{3:x2},0x{4:x2}={5:D}>",
                    expected[10], expected[11], BitConverter.ToUInt16(expected, 10),
                    actual[10], actual[11], BitConverter.ToUInt16(actual, 10)) , e);
            }
            try
            {
                Assert.AreEqual(expected[12], actual[12], "Offset 12, wTime2 lo");
                Assert.AreEqual(expected[13], actual[13], "Offset 13, wTime2 hi");
            }
            catch (AssertFailedException e)
            {
                throw new AssertFailedException(string.Format("Expected <0x{0:x2},0x{1:x2}={2:D}> Actual <0x{3:x2},0x{4:x2}={5:D}>",
                    expected[12], expected[13], BitConverter.ToUInt16(expected, 12),
                    actual[12], actual[13], BitConverter.ToUInt16(actual, 12)) , e);
            }
            Assert.AreEqual(expected[14], actual[14], "Offset 14, CtrlVal0");
            Assert.AreEqual(expected[15], actual[15], "Offset 15, CtrlVal1");
        }
    }
}
