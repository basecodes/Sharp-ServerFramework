local Classes = require "Classes"

local Factory = Classes.class() 

function Factory:init()
	self.ArrayBase = FieldType.ArrayBase
	self.ArrayPacket = FieldType.ArrayPacket
	self.DictKBVB = FieldType.DictKBVB
	self.DictKBVP = FieldType.DictKBVP
	self.DictKPVP = FieldType.DictKPVP
	self.DictKPVB = FieldType.DictKPVB
end

function Factory:Create(type)
	return {Type = type}
end

return Factory