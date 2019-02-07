local classes = require "Classes"

local ControllerComponent = classes.class()

function ControllerComponent:init(child)
	self.IControllerComponent = LuaProxy.NewRpcComponent(child,LuaHelper)
end

return RpcComponent