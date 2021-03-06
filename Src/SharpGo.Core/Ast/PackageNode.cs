﻿namespace SharpGo.Core.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class PackageNode : IStatementNode
    {
        private readonly string name;

        public PackageNode(string name)
        {
            this.name = name;
        }

        public string Name { get { return this.name; } }
    }
}
