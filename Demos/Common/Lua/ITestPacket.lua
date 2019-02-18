local Classes = require "Classes"
local packet =  require "SerializablePacket"
local ITestPacket = Classes.class(packet)

function ITestPacket:init(child)
	self.super:init(child,"ITestPacket")
	self.Name = ""
	self.Password = ""

	local mt = {}
	mt.__tostring = function(packet)
		return packet.Name .. " " .. packet.Password
	end
	setmetatable(self,mt)
end

function ITestPacket:FromBinaryReader( reader )

end

function ITestPacket:ToBinaryWriter( writer )

end

return ITestPacket