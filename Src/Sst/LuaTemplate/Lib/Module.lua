local Classes = require "Classes"

local Module = Classes.class()

function Module:init(child)
	self.IModule = LuaProxy:CreateModule(child,LuaHelper)

	self.ModuleName = ""
	self.ServiceId = ""
end

-- abstract
function Module:Initialize(server,cacheManager,controllerComponentManager)
	-- Body
end

-- abstract
function Module:InitFinish(server,cacheManager,controllerComponentManager)
	-- Body
end

-- abstract
function Module:Dispose(cacheManager,controllerComponentManager)
	-- Body
end

-- abstract
function Module:Dispose()
	-- Body
end

-- abstract
function Module:Finish(server,cacheManager,controllerComponentManager)
	-- Body
end

-- abstract
function Module:Accepted(peer,readStream,writeStream)
	-- Body
	return true
end

-- abstract
function Module:Connected(peer,readStream)
	-- Body
end

-- abstract
function Module:Disconnected(peer)
	-- Body
end

function Module:AddController(func)
	self.IModule:AddController(func)
end

function Module:AddPacket(interface,func)
	self.IModule:AddPacket(interface,func)
end

return Module