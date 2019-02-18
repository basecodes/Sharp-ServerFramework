local classes = require "Classes"

local SerializablePacket = classes.class()

function SerializablePacket:init(child,interface)
  self.ISerializablePacket = LuaProxy.CreatePacket(interface,child,LuaHelper)

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

  self.Type = FieldType.PacketType
end

function SerializablePacket:GetObject(interface)
  return self.ISerializablePacket.GetObject(interface)
end

function SerializablePacket:Recycle(interface,packet)
  self.ISerializablePacket.Recycle(interface,packet.ISerializablePacket)
end

function SerializablePacket:FromBinaryReader( reader )
end

function SerializablePacket:ToBinaryWriter( writer )
end

function SerializablePacket:Assign()
end

function SerializablePacket:Recycle()
end

return SerializablePacket