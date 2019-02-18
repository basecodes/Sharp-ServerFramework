local Classes = require "Classes"
local packet =  require "SerializablePacket"
local BaseType = require "BaseType"
local Factory = require "Factory"

local ITestPacket = Classes.class(packet)

function ITestPacket:init(child)
	self.super:init(child,"ITestPacket")
	self.Name = Factory:CreateBase(BaseType.string)
	self.Password = Factory:CreateBase(BaseType.string)

	local mt = {}
	mt.__tostring = function(packet)
		return packet.Name.Value .. " " .. packet.Password.Value
	end
	setmetatable(self,mt)
end

function ITestPacket:FromBinaryReader( reader )

end

function ITestPacket:ToBinaryWriter( writer )

end

return ITestPacket