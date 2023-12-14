using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ProjectNostalgia.combat;

namespace ProjectNostalgia.interfaces;

public interface IComponentContainer
{

    public ComponentManager GetManager();

}