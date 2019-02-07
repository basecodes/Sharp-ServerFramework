local Classes = require "Classes"

local Module = Classes.class()

function Module:init(child,id)
	self.IModule = LuaProxy:New(child,id,LuaHelper)
end

-- abstract
function Module:Initialize(server,cacheManager,rpcComponentManager)
	-- Body
end

-- abstract
function Module:InitFinish(server,cacheManager,rpcComponentManager)
	-- Body
end

-- abstract
function Module:Dispose(cacheManager,rpcComponentManager)
	-- Body
end

-- abstract
function Module:Dispose()
	-- Body
end

-- abstract
function Module:Finish(server,cacheManager,rpcComponentManager)
	-- Body
end

-- abstract
function Module:Accepted(peer,readStream,writeStream)
	-- Body
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