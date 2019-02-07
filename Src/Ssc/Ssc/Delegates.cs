using System;
using System.Collections;
using Ssc.Ssc;
using Ssc.SscLog;
using Ssc.SscSerialization;

public delegate T ObjectActivator<T>(params object[] arguments);

public delegate void Delegates(bool isSuccessful, string error);

public delegate void PermissionLevelCallback(int? permissionLevel, string error);

public delegate void ResponseCallback(IResponseMessage responseMessage,IDeserializable deserializable);

public delegate bool LogHandler(
    Logger logger,
    LogLevel logLevel,
    object message,
    string callingFilePath,
    string callingMethod,
    int callingFileLineNumber);