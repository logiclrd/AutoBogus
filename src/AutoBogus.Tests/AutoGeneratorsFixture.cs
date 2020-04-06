using AutoBogus.Generators;
using AutoBogus.Tests.Models.Simple;
using AutoBogus.Util;
using Bogus;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AutoBogus.Tests
{
  public partial class AutoGeneratorsFixture
  {
    public class RegisteredGenerator
      : AutoGeneratorsFixture
    {
      [Theory]
      [MemberData(nameof(GetRegisteredTypes))]
      public void Generate_Should_Return_Value(Type type)
      {
        var generator = AutoGeneratorFactory.Generators[type];

        InvokeGenerator(type, generator).Should().BeOfType(type);
      }

      [Theory]
      [MemberData(nameof(GetRegisteredTypes))]
      public void GetGenerator_Should_Return_Generator(Type type)
      {
        var context = CreateContext(type);
        var generator = AutoGeneratorFactory.Generators[type];

        AutoGeneratorFactory.GetGenerator(context).Should().Be(generator);
      }

      public static IEnumerable<object[]> GetRegisteredTypes()
      {
        return AutoGeneratorFactory.Generators.Select(g => new object[]
        {
          g.Key
        });
      }
    }

    public class ArrayGenerator
      : AutoGeneratorsFixture
    {
      [Theory]
      [InlineData(typeof(TestEnum[]))]
      [InlineData(typeof(TestStruct[]))]
      [InlineData(typeof(TestClass[]))]
      [InlineData(typeof(ITestInterface[]))]
      [InlineData(typeof(TestAbstractClass[]))]
      public void Generate_Should_Return_Array(Type type)
      {
        var itemType = type.GetElementType();
        var generator = CreateGenerator(typeof(ArrayGenerator<>), itemType);
        var array = InvokeGenerator(type, generator) as Array;

        array.Should().NotBeNull().And.NotContainNulls();
      }

      [Theory]
      [InlineData(typeof(TestEnum[]))]
      [InlineData(typeof(TestStruct[]))]
      [InlineData(typeof(TestClass[]))]
      [InlineData(typeof(ITestInterface[]))]
      [InlineData(typeof(TestAbstractClass[]))]
      public void GetGenerator_Should_Return_ArrayGenerator(Type type)
      {
        var context = CreateContext(type);
        var itemType = type.GetElementType();
        var generatorType = GetGeneratorType(typeof(ArrayGenerator<>), itemType);

        AutoGeneratorFactory.GetGenerator(context).Should().BeOfType(generatorType);
      }
    }

    public class EnumGenerator
      : AutoGeneratorsFixture
    {
      [Fact]
      public void Generate_Should_Return_Enum()
      {
        var type = typeof(TestEnum);
        var generator = new EnumGenerator<TestEnum>();

        InvokeGenerator(type, generator).Should().BeOfType<TestEnum>();
      }

      [Fact]
      public void GetGenerator_Should_Return_EnumGenerator()
      {
        var type = typeof(TestEnum);
        var context = CreateContext(type);

        AutoGeneratorFactory.GetGenerator(context).Should().BeOfType<EnumGenerator<TestEnum>>();
      }
    }

    public class DictionaryGenerator
      : AutoGeneratorsFixture
    {
      [Theory]
      [InlineData(typeof(IDictionary<int, TestEnum>))]
      [InlineData(typeof(IDictionary<int, TestStruct>))]
      [InlineData(typeof(IDictionary<int, TestClass>))]
      [InlineData(typeof(IDictionary<int, ITestInterface>))]
      [InlineData(typeof(IDictionary<int, TestAbstractClass>))]
      [InlineData(typeof(Dictionary<int, TestClass>))]
      [InlineData(typeof(SortedList<int, TestClass>))]
      public void Generate_Should_Return_Dictionary(Type type)
      {
        var genericTypes = ReflectionHelper.GetGenericArguments(type);
        var keyType = genericTypes.ElementAt(0);
        var valueType = genericTypes.ElementAt(1);
        var generator = CreateGenerator(typeof(DictionaryGenerator<,>), keyType, valueType);
        var dictionary = InvokeGenerator(type, generator) as IDictionary;

        dictionary.Should().NotBeNull().And.NotContainNulls();

        foreach (var key in dictionary.Keys)
        {
          var value = dictionary[key];

          key.Should().BeOfType(keyType);
          value.Should().BeOfType(valueType);
        }
      }

      [Theory]
      [InlineData(typeof(IDictionary<int, TestEnum>))]
      [InlineData(typeof(IDictionary<int, TestStruct>))]
      [InlineData(typeof(IDictionary<int, TestClass>))]
      [InlineData(typeof(IDictionary<int, ITestInterface>))]
      [InlineData(typeof(IDictionary<int, TestAbstractClass>))]
      [InlineData(typeof(Dictionary<int, TestClass>))]
      [InlineData(typeof(SortedList<int, TestClass>))]
      public void GetGenerator_Should_Return_DictionaryGenerator(Type type)
      {
        var context = CreateContext(type);
        var genericTypes = ReflectionHelper.GetGenericArguments(type);
        var keyType = genericTypes.ElementAt(0);
        var valueType = genericTypes.ElementAt(1);
        var generatorType = GetGeneratorType(typeof(DictionaryGenerator<,>), keyType, valueType);

        AutoGeneratorFactory.GetGenerator(context).Should().BeOfType(generatorType);
      }
    }

    public class SetGenerator
      : AutoGeneratorsFixture
    {
      [Theory]
      [InlineData(typeof(ISet<TestEnum>))]
      [InlineData(typeof(ISet<TestStruct>))]
      [InlineData(typeof(ISet<TestClass>))]
      [InlineData(typeof(ISet<ITestInterface>))]
      [InlineData(typeof(ISet<TestAbstractClass>))]
      [InlineData(typeof(HashSet<TestClass>))]
      public void Generate_Should_Return_Set(Type type)
      {
        var genericTypes = ReflectionHelper.GetGenericArguments(type);
        var itemType = genericTypes.ElementAt(0);
        var generator = CreateGenerator(typeof(SetGenerator<>), itemType);
        var set = InvokeGenerator(type, generator) as IEnumerable;

        set.Should().NotBeNull().And.NotContainNulls();
      }

      [Theory]
      [InlineData(typeof(ISet<TestEnum>))]
      [InlineData(typeof(ISet<TestStruct>))]
      [InlineData(typeof(ISet<TestClass>))]
      [InlineData(typeof(ISet<ITestInterface>))]
      [InlineData(typeof(ISet<TestAbstractClass>))]
      [InlineData(typeof(HashSet<TestClass>))]
      public void GetGenerator_Should_Return_SetGenerator(Type type)
      {
        var context = CreateContext(type);
        var genericTypes = ReflectionHelper.GetGenericArguments(type);
        var itemType = genericTypes.ElementAt(0);
        var generatorType = GetGeneratorType(typeof(SetGenerator<>), itemType);

        AutoGeneratorFactory.GetGenerator(context).Should().BeOfType(generatorType);
      }
    }

    public class EnumerableGenerator
      : AutoGeneratorsFixture
    {
      [Theory]
      [InlineData(typeof(IEnumerable<TestEnum>))]
      [InlineData(typeof(IEnumerable<TestStruct>))]
      [InlineData(typeof(IEnumerable<TestClass>))]
      [InlineData(typeof(IEnumerable<ITestInterface>))]
      [InlineData(typeof(IEnumerable<TestAbstractClass>))]
      [InlineData(typeof(ICollection<TestAbstractClass>))]
      [InlineData(typeof(List<TestClass>))]
      public void Generate_Should_Return_Enumerable(Type type)
      {
        var genericTypes = ReflectionHelper.GetGenericArguments(type);
        var itemType = genericTypes.ElementAt(0);
        var generator = CreateGenerator(typeof(EnumerableGenerator<>), itemType);
        var enumerable = InvokeGenerator(type, generator) as IEnumerable;

        enumerable.Should().NotBeNull().And.NotContainNulls();
      }

      [Theory]
      [InlineData(typeof(IEnumerable<TestEnum>))]
      [InlineData(typeof(IEnumerable<TestStruct>))]
      [InlineData(typeof(IEnumerable<TestClass>))]
      [InlineData(typeof(IEnumerable<ITestInterface>))]
      [InlineData(typeof(IEnumerable<TestAbstractClass>))]
      [InlineData(typeof(ICollection<TestAbstractClass>))]
      [InlineData(typeof(List<TestClass>))]
      public void GetGenerator_Should_Return_EnumerableGenerator(Type type)
      {
        var context = CreateContext(type);
        var genericTypes = ReflectionHelper.GetGenericArguments(type);
        var itemType = genericTypes.ElementAt(0);
        var generatorType = GetGeneratorType(typeof(EnumerableGenerator<>), itemType);

        AutoGeneratorFactory.GetGenerator(context).Should().BeOfType(generatorType);
      }
    }

    public class NullableGenerator
      : AutoGeneratorsFixture
    {
      [Fact]
      public void Generate_Should_Return_Value()
      {
        var type = typeof(TestEnum?);
        var generator = new NullableGenerator<TestEnum>();

        InvokeGenerator(type, generator).Should().BeOfType<TestEnum>();
      }

      [Fact]
      public void GetGenerator_Should_Return_NullableGenerator()
      {
        var type = typeof(TestEnum?);
        var context = CreateContext(type);

        AutoGeneratorFactory.GetGenerator(context).Should().BeOfType<NullableGenerator<TestEnum>>();
      }
    }

    public class TypeGenerator
      : AutoGeneratorsFixture
    {
      [Theory]
      [InlineData(typeof(TestStruct))]
      [InlineData(typeof(TestClass))]
      [InlineData(typeof(ITestInterface))]
      [InlineData(typeof(TestAbstractClass))]
      public void Generate_Should_Return_Value(Type type)
      {
        var generator = CreateGenerator(typeof(TypeGenerator<>), type);

        if (ReflectionHelper.IsInterface(type) || ReflectionHelper.IsAbstract(type))
        {
          InvokeGenerator(type, generator).Should().BeNull();
        }
        else
        {
          InvokeGenerator(type, generator).Should().BeAssignableTo(type);
        }
      }

      [Theory]
      [InlineData(typeof(TestStruct))]
      [InlineData(typeof(TestClass))]
      [InlineData(typeof(ITestInterface))]
      [InlineData(typeof(TestAbstractClass))]
      public void GetGenerator_Should_Return_TypeGenerator(Type type)
      {
        var context = CreateContext(type);
        var generatorType = GetGeneratorType(typeof(TypeGenerator<>), type);

        AutoGeneratorFactory.GetGenerator(context).Should().BeOfType(generatorType);
      }
    }

    public class VersionGenerator
      : AutoGeneratorsFixture
    {
      [Fact]
      public void Should_Populate_Version_Object()
      {
        var version = AutoFaker.Generate<Version>();

        version.Major.Should().BeGreaterOrEqualTo(0);
        version.Minor.Should().BeGreaterOrEqualTo(0);
        version.Build.Should().BeGreaterOrEqualTo(0);
        version.Revision.Should().BeGreaterOrEqualTo(0);
      }
    }

    public class GeneratorOverrides
      : AutoGeneratorsFixture
    {
      private AutoGeneratorOverride _generatorOverride;
      private IList<AutoGeneratorOverride> _overrides;

      private class TestGeneratorOverride
        : AutoGeneratorOverride
      {
        public TestGeneratorOverride(bool shouldOverride = false)
        {
          ShouldOverride = shouldOverride;
        }

        public bool ShouldOverride { get; }

        public override bool CanOverride(AutoGenerateContext context)
        {
          return ShouldOverride;
        }

        public override void Generate(AutoGenerateOverrideContext context)
        { }
      }

      public GeneratorOverrides()
      {
        _generatorOverride = new TestGeneratorOverride(true);
        _overrides = new List<AutoGeneratorOverride>
        {
          new TestGeneratorOverride(),
          _generatorOverride,
          new TestGeneratorOverride()
        };
      }

      [Fact]
      public void Should_Return_All_Matching_Overrides()
      {
        var generatorOverride = new TestGeneratorOverride(true);

        _overrides.Insert(1, generatorOverride);

        var context = CreateContext(typeof(string), _overrides);
        var invoker = AutoGeneratorFactory.GetGenerator(context) as AutoGeneratorOverrideInvoker;

        invoker.Overrides.Should().BeEquivalentTo(new[] { generatorOverride, _generatorOverride });
      }

      [Fact]
      public void Should_Return_Generator_If_No_Matching_Override()
      {
        _overrides = new List<AutoGeneratorOverride>
        {
          new TestGeneratorOverride()
        };

        var context = CreateContext(typeof(int), _overrides);
        AutoGeneratorFactory.GetGenerator(context).Should().BeOfType<IntGenerator>();
      }

      [Fact]
      public void Should_Invoke_Generator()
      {
        var context = CreateContext(typeof(string), _overrides);
        var generatorOverride = AutoGeneratorFactory.GetGenerator(context);

        generatorOverride.Generate(context).Should().BeOfType<string>().And.Should().NotBeNull();
      }
    }

    private object InvokeGenerator(Type type, IAutoGenerator generator)
    {
      var context = CreateContext(type);
      return generator.Generate(context);
    }

    private static Type GetGeneratorType(Type type, params Type[] types)
    {
      return type.MakeGenericType(types);
    }

    private static IAutoGenerator CreateGenerator(Type type, params Type[] types)
    {
      type = GetGeneratorType(type, types);
      return (IAutoGenerator)Activator.CreateInstance(type);
    }

    private AutoGenerateContext CreateContext(Type type, IList<AutoGeneratorOverride> generatorOverrides = null)
    {
      var faker = new Faker();
      var config = new AutoConfig();

      if (generatorOverrides != null)
      {
        config.Overrides = generatorOverrides;
      }

      return new AutoGenerateContext(faker, config)
      {
        GenerateType = type
      };
    }
  }
}
