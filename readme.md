About
-----

Expressive is a library that converts method IL into expression trees.  
Or, at least, it attempts to, since not all methods can be converted into a single expression.

Example:

    public string FullName {
        get { return this.FirstName + " " + this.LastName; }
    }

is converted to

    x => string.Concat(x.FirstName, " ", x.LastName)

API
---
https://github.com/ashmind/expressive/wiki/External-API

Reason
------

There are situations (mostly LINQ-related) when there is a large difference between use of a simple (for example, DB-mapped) property and a calculated property that contains some business/domain logic. Above example is the most classical one, but there are a lot more.

These cases can be solved using things like HqlGeneratorForProperty in NHibernate and rewriters/visitors in other frameworks, but this requires logic/effort duplication, and such logic has to be maintained in both places. 

Expressive allows you to reuse the code directly. In addition to queries, other benefits of such reuse can be, for example, automatic generation of formula columns or transfer of logic between compiled C# and JavaScript.

Issues
------

All code that uses Expressive should be used in combination with integration tests since significant changes to methods may render them 'inexpressible' (there are some cases when statements can be reduced to expressions, but these cases are rather limited). One rather annoying breaking case is IL instrumentation by, for example, Visual Studio Profiler.

Status
------

What already works:

1. Fields/Properties
2. Methods
3. Operators
4. Lambdas
5. Object initializers (only one level deep)
6. Simple `if` that can be expressed as `?:`

What does not work:

1. Array item access
2. Deep object initializers
3. Statements

Note: it is completely possible to allow .NET 4.0 "statement expressions" in the results by changing the default Pipeline and replacing a Decompiler.