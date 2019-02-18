local Classes = require "Classes"
local ITestPacket = require "ITestPacket"
local BaseType = require "BaseType"

local TestPacket = Classes.class(ITestPacket)

function TestPacket:init()
	self.super:init(self)
end

function TestPacket:FromBinaryReader( reader )
	self.Name.Value = reader:Read()
	self.Password.Value = reader:Read()
end

function TestPacket:ToBinaryWriter( writer )
	writer:Write(BaseType.string,self.Name)
	writer:Write(BaseType.string,self.Password)
end

return TestPacket