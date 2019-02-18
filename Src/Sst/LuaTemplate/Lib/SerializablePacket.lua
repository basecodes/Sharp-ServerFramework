local classes = require "Classes"

local SerializablePacket = classes.class()

function SerializablePacket:init(child,interface)
  self.ISerializablePacket = LuaProxy.CreatePacket(interface,child,LuaHelper)
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