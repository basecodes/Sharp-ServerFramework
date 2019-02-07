local Classes = require "Classes"
local packet =  require "SerializablePacket"
local ITestPacket = Classes.class(packet)

function ITestPacket:init(child)
	self.super:init(child,"ITestPacket")
	self.Name = ""
	self.Password = ""
end

function ITestPacket:FromBinaryReader( reader )

end

function ITestPacket:ToBinaryWriter( writer )

end

function ITestPacket:ToString()
	return self.Name .. " " .. self.Password
end

return ITestPacket