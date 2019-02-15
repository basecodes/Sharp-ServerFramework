local Classes = require "Classes"
local Module = require "Module"
local TestController = require "TestController"
local Loader = require "Loader"
local TestPacket = require "TestPacket"

local TestModule = Classes.class(Module)

function TestModule:init()
	self.super:init(self)

	self.ModuleName = "TestModule"
	self.ServiceId = "Test"
end

function TestModule:Initialize(server,cacheManager,controllerComponentManager)
	self.super:Initialize(server,cacheManager,controllerComponentManager)

    self:AddController(function() 
		return TestController.new()
	end)

	self:AddPacket("ITestPacket",function()
		return TestPacket.new()
	end)
end

function TestModule:Finish(server,cacheManager,controllerComponentManager )
	self.super:Finish(server,cacheManager,controllerComponentManager)
end

function TestModule:Connected(peer,readStream)
	self.super:Connected(peer,readStream)
end

function TestModule:Disconnected(peer)
	self.super:Disconnected(peer)
end

return TestModule