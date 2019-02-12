local classes = require "Classes"

local Controller = classes.class()

function Controller:init(child)
	self.IController = LuaProxy:New(child)
end

function Controller:Register(key,func)
	local id = LuaProxy:Register(self,key,func)

	if self.MethodIds == nil then
		self.MethodIds = id
	else
		self.MethodIds = self.MethodIds .. ";" .. id
	end
end

function Controller:Invoke(methodId,peer,callback,...)
	LuaProxy:Invoke(methodId,peer,callback,...)
end

return Controller