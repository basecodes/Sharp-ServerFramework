local Classes = require "Classes"
local Module = require "Module"
local TestController = require "TestController"
local Loader = require "Loader"
local TestPacket = require "TestPacket"

local TestModule = Classes.class(Module)

function TestModule:init()
	self.super:init(self,"Test") -- ServiceId = "Test"
end

function TestModule:Initialize(server,cacheManager,rpcComponentManager)
	self.super:Initialize(server,cacheManager,rpcComponentManager)

    self:AddController(function() 
		return TestController.new()
	end)

	self:AddPacket("ITestPacket",function()
		return TestPacket.new()
	end)
end

function TestModule:Finish(server,cacheManager,rpcComponentManager )
	self.super:Finish(server,cacheManager,rpcComponentManager)
end

function TestModule:Connected( peer ,readStream)
	self.super:Connected(peer,readStream)
end

function TestModule:Disconnected( peer )
	self.super:Disconnected(peer)
end

return TestModule