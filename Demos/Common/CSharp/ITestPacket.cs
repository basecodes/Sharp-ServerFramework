using System;
using System.Collections.Generic;
using System.Text;
using Ssc.SscSerialization;
using Ssc.Ssc;

public interface ITestPacket : ISerializablePacket {
    string Name { get; set; }
    string Password { get; set; }
}