local Classes = require "Classes"
local ITestPacket = require "ITestPacket"

local TestPacket = Classes.class(ITestPacket)

function TestPacket:init()
	self.super:init(self)
end

function TestPacket:FromBinaryReader( reader )
	self.Name = reader:Read()
	self.Password = reader:Read()
end

function TestPacket:ToBinaryWriter( writer )
	writer:Write(self.string,self.Name)
	writer:Write(self.string,self.Password)
end

return TestPacket