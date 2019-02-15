local Classes = require "Classes"

local Factory = Classes.class() 

function Factory:init()
	self.NullType = FieldType.NullType
	self.BaseType = FieldType.BaseType
	self.PacketType = FieldType.PacketType
	self.ArrayBase = FieldType.ArrayBase
	self.ArrayPacket = FieldType.ArrayPacket
	self.DictKBVB = FieldType.DictKBVB
	self.DictKBVP = FieldType.DictKBVP
	self.DictKPVP = FieldType.DictKPVP
	self.DictKPVB = FieldType.DictKPVB
end

function Factory:Create(type)
	return {Type = type,Value = nil}
end

return Factory