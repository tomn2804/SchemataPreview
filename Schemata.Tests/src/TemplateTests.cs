﻿using System;
using System.Collections.Generic;
using Xunit;

namespace Schemata.Tests;

public abstract class TemplateTests<T> where T : Template
{
    public abstract void ToBlueprint_BasicCase_ReturnsBlueprint();

    public abstract void ToBlueprint_FromDynamicComposition_ReturnsBlueprint();
}
