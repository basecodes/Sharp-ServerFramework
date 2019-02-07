local classes = require "Classes"

local SerializablePacket = classes.class()

function SerializablePacket:init(child,interface)
  self.ISerializablePacket = LuaProxy.New(interface,child,LuaHelper)

  self.char = TypeCode.Char
  self.bool = TypeCode.Boolean
  self.sbyte = TypeCode.SByte
  self.byte = TypeCode.Byte
  self.short = TypeCode.Int16
  self.ushort = TypeCode.UInt16
  self.int = TypeCode.Int32
  self.uint = TypeCode.UInt32
  self.long = TypeCode.Int64
  self.ulong = TypeCode.UInt64
  self.float = TypeCode.Single
  self.double = TypeCode.Double
  self.string = TypeCode.String
end

function SerializablePacket:GetObject(interface)
  return LuaProxy.GetObject(interface)
end

function SerializablePacket:Recycle(interface,packet)
  LuaProxy.Recycle(interface,packet.ISerializablePacket)
end

return SerializablePacket