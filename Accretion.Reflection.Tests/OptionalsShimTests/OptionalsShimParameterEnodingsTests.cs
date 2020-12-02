using Accretion.Reflection.Emit;
using System;
using Xunit;
using static ILMethodsWithDefaultParameters;

namespace Accretion.Reflection.Tests
{
    public class OptionalsShimParameterEnodingsTests
    {
        [Fact]
        public void SupportsProperlyEncodedBooleanConstants()
        {
            var source = typeof(ILMethodsWithDefaultParameters).GetMethod(nameof(BooleansEncodedProperly));
            var shim = Shim.Create<Func<bool>>(source);
            Assert.True(shim());
        }

        [Fact]
        public void DoesNotSupportBadlyEncodedBooleanConstants()
        {
            var type = typeof(ILMethodsWithDefaultParameters);

            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(BooleanEncodedWithChar))));
            
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(BooleanEncodedWithChar))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(BooleanEncodedWithTooSmallSByte))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(BooleanEncodedWithTooBigSByte))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(BooleanEncodedWithTooBigByte))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(BooleanEncodedWithTooSmallInt16))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(BooleanEncodedWithTooBigInt16))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(BooleanEncodedWithTooBigUInt16))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(BooleanEncodedWithTooSmallInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(BooleanEncodedWithTooBigInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(BooleanEncodedWithTooBigUInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(BooleanEncodedWithTooSmallInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(BooleanEncodedWithTooBigInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(BooleanEncodedWithTooBigUInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(BooleanEncodedWithSingle))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(BooleanEncodedWithDouble))));
        }

        [Fact]
        public void SupportsProperlyEncodedCharConstants()
        {
            var source = typeof(ILMethodsWithDefaultParameters).GetMethod(nameof(CharsEncodedProperly));
            var shim = Shim.Create<Func<bool>>(source);
            Assert.True(shim());
        }

        [Fact]
        public void DoesNotSupportBadlyEncodedCharConstants()
        {
            var type = typeof(ILMethodsWithDefaultParameters);
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(CharEncodedWithBoolean))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(CharEncodedWithTooSmallSByte))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(CharEncodedWithTooSmallInt16))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(CharEncodedWithTooSmallInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(CharEncodedWithTooBigInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(CharEncodedWithTooBigUInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(CharEncodedWithTooSmallInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(CharEncodedWithTooBigInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(CharEncodedWithTooBigUInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(CharEncodedWithSingle))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(CharEncodedWithDouble))));
        }

        [Fact]
        public void SupportsProperlyEncodedSingleConstants()
        {
            var source = typeof(ILMethodsWithDefaultParameters).GetMethod(nameof(SinglesEncodedProperly));
            var shim = Shim.Create<Func<bool>>(source);
            Assert.True(shim());
        }

        [Fact]
        public void DoesNotSupportBadlyEncodedSingleConstants()
        {
            var type = typeof(ILMethodsWithDefaultParameters);
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(SingleEncodedWithBoolean))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(SingleEncodedWithChar))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(SingleEncodedWithSByte))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(SingleEncodedWithByte))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(SingleEncodedWithInt16))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(SingleEncodedWithUInt16))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(SingleEncodedWithInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(SingleEncodedWithUInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(SingleEncodedWithInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(SingleEncodedWithUInt64))));

        }

        [Fact]
        public void SupportsProperlyEncodedDoubleConstants()
        {
            var source = typeof(ILMethodsWithDefaultParameters).GetMethod(nameof(DoublesEncodedProperly));
            var shim = Shim.Create<Func<bool>>(source);
            Assert.True(shim());
        }

        [Fact]
        public void DoesNotSupportBadlyEncodedDoubleConstants()
        {
            var type = typeof(ILMethodsWithDefaultParameters);
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(DoubleEncodedWithBoolean))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(DoubleEncodedWithChar))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(DoubleEncodedWithSingle))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(DoubleEncodedWithSByte))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(DoubleEncodedWithByte))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(DoubleEncodedWithInt16))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(DoubleEncodedWithUInt16))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(DoubleEncodedWithInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(DoubleEncodedWithUInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(DoubleEncodedWithInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(DoubleEncodedWithUInt64))));
        }

        [Fact]
        public void SupportsProperlyEncodedSByteConstants()
        {
            var source = typeof(ILMethodsWithDefaultParameters).GetMethod(nameof(SBytesEncodedProperly));
            var shim = Shim.Create<Func<bool>>(source);
            Assert.True(shim());
        }

        [Fact]
        public void DoesNotSupportBadlyEncodedSByteConstants()
        {
            var type = typeof(ILMethodsWithDefaultParameters);
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(SByteEncodedWithBoolean))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(SByteEncodedWithChar))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(SByteEncodedWithTooBigByte))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(SByteEncodedWithTooSmallInt16))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(SByteEncodedWithTooBigInt16))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(SByteEncodedWithTooBigUInt16))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(SByteEncodedWithTooSmallInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(SByteEncodedWithTooBigInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(SByteEncodedWithTooBigUInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(SByteEncodedWithTooSmallInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(SByteEncodedWithTooBigInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(SByteEncodedWithTooBigUInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(SByteEncodedWithSingle))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(SByteEncodedWithDouble))));
        }

        [Fact]
        public void SupportsProperlyEncodedByteConstants()
        {
            var source = typeof(ILMethodsWithDefaultParameters).GetMethod(nameof(BytesEncodedProperly));
            var shim = Shim.Create<Func<bool>>(source);
            Assert.True(shim());
        }

        [Fact]
        public void DoesNotSupportBadlyEncodedByteConstants()
        {
            var type = typeof(ILMethodsWithDefaultParameters);
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(ByteEncodedWithBoolean))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(ByteEncodedWithChar))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(ByteEncodedWithTooSmallSByte))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(ByteEncodedWithTooSmallInt16))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(ByteEncodedWithTooBigInt16))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(ByteEncodedWithTooBigUInt16))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(ByteEncodedWithTooSmallInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(ByteEncodedWithTooBigInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(ByteEncodedWithTooBigUInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(ByteEncodedWithTooSmallInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(ByteEncodedWithTooBigInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(ByteEncodedWithTooBigUInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(ByteEncodedWithSingle))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(ByteEncodedWithDouble))));
        }

        [Fact]
        public void SupportsProperlyEncodedInt16Constants()
        {
            var source = typeof(ILMethodsWithDefaultParameters).GetMethod(nameof(Int16sEncodedProperly));
            var shim = Shim.Create<Func<bool>>(source);
            Assert.True(shim());
        }

        [Fact]
        public void DoesNotSupportBadlyEncodedInt16Constants()
        {
            var type = typeof(ILMethodsWithDefaultParameters);
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(Int16EncodedWithBoolean))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(Int16EncodedWithChar))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(Int16EncodedWithTooBigUInt16))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(Int16EncodedWithTooSmallInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(Int16EncodedWithTooBigInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(Int16EncodedWithTooBigUInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(Int16EncodedWithTooSmallInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(Int16EncodedWithTooBigInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(Int16EncodedWithTooBigUInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(Int16EncodedWithSingle))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(Int16EncodedWithDouble))));
        }

        [Fact]
        public void SupportsProperlyEncodedUInt16Constants()
        {
            var source = typeof(ILMethodsWithDefaultParameters).GetMethod(nameof(UInt16sEncodedProperly));
            var shim = Shim.Create<Func<bool>>(source);
            Assert.True(shim());
        }

        [Fact]
        public void DoesNotSupportBadlyEncodedUInt16Constants()
        {
            var type = typeof(ILMethodsWithDefaultParameters);
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt16EncodedWithBoolean))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt16EncodedWithChar))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt16EncodedWithTooSmallSByte))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt16EncodedWithTooSmallInt16))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt16EncodedWithTooSmallInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt16EncodedWithTooBigInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt16EncodedWithTooBigUInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt16EncodedWithTooSmallInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt16EncodedWithTooBigInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt16EncodedWithTooBigUInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt16EncodedWithSingle))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt16EncodedWithDouble))));
        }

        [Fact]
        public void SupportsProperlyEncodedInt32Constants()
        {
            var source = typeof(ILMethodsWithDefaultParameters).GetMethod(nameof(Int32sEncodedProperly));
            var shim = Shim.Create<Func<bool>>(source);
            Assert.True(shim());
        }

        [Fact]
        public void DoesNotSupportBadlyEncodedInt32Constants()
        {
            var type = typeof(ILMethodsWithDefaultParameters);
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(Int32EncodedWithBoolean))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(Int32EncodedWithChar))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(Int32EncodedWithTooBigUInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(Int32EncodedWithTooSmallInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(Int32EncodedWithTooBigInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(Int32EncodedWithTooBigUInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(Int32EncodedWithSingle))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(Int32EncodedWithDouble))));
        }

        [Fact]
        public void SupportsProperlyEncodedUInt32Constants()
        {
            var source = typeof(ILMethodsWithDefaultParameters).GetMethod(nameof(UInt32sEncodedProperly));
            var shim = Shim.Create<Func<bool>>(source);
            Assert.True(shim());
        }

        [Fact]
        public void DoesNotSupportBadlyEncodedUInt32Constants()
        {
            var type = typeof(ILMethodsWithDefaultParameters);
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt32EncodedWithBoolean))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt32EncodedWithChar))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt32EncodedWithTooSmallSByte))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt32EncodedWithTooSmallInt16))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt32EncodedWithTooSmallInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt32EncodedWithTooSmallInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt32EncodedWithTooBigInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt32EncodedWithTooBigUInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt32EncodedWithSingle))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt32EncodedWithDouble))));
        }

        [Fact]
        public void SupportsProperlyEncodedInt64Constants()
        {
            var source = typeof(ILMethodsWithDefaultParameters).GetMethod(nameof(Int64sEncodedProperly));
            var shim = Shim.Create<Func<bool>>(source);
            Assert.True(shim());
        }

        [Fact]
        public void DoesNotSupportBadlyEncodedInt64Constants()
        {
            var type = typeof(ILMethodsWithDefaultParameters);
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(Int64EncodedWithBoolean))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(Int64EncodedWithChar))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(Int64EncodedWithTooBigUInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(Int64EncodedWithSingle))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(Int64EncodedWithDouble))));
        }

        [Fact]
        public void SupportsProperlyEncodedUInt64Constants()
        {
            var source = typeof(ILMethodsWithDefaultParameters).GetMethod(nameof(UInt64sEncodedProperly));
            var shim = Shim.Create<Func<bool>>(source);
            Assert.True(shim());
        }

        [Fact]
        public void DoesNotSupportBadlyEncodedUInt64Constants()
        {
            var type = typeof(ILMethodsWithDefaultParameters);
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt64EncodedWithBoolean))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt64EncodedWithChar))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt64EncodedWithTooSmallSByte))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt64EncodedWithTooSmallInt16))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt64EncodedWithTooSmallInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt64EncodedWithTooSmallInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt64EncodedWithSingle))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(UInt64EncodedWithDouble))));
        }

        [Fact]
        public void SupportsProperlyEncodedNativeIntegerConstants()
        {
            var source = typeof(ILMethodsWithDefaultParameters).GetMethod(nameof(NativeIntsEncodedProperly));
            var shim = Shim.Create<Func<bool>>(source);
            Assert.True(shim());
        }

        [Fact]
        public void DoesNotSupportBadlyEncodedNativeIntegerConstants()
        {
            var type = typeof(ILMethodsWithDefaultParameters);
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(NativeIntEncodedWithBoolean))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(NativeIntEncodedWithChar))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(NativeIntEncodedWithTooBigUInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(NativeIntEncodedWithTooSmallInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(NativeIntEncodedWithTooBigInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(NativeIntEncodedWithTooBigUInt64))));

            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(NativeUIntEncodedWithBoolean))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(NativeUIntEncodedWithChar))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(NativeUIntEncodedWithTooSmallInt8))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(NativeUIntEncodedWithTooSmallInt16))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(NativeUIntEncodedWithTooSmallInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(NativeUIntEncodedWithTooSmallInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(NativeUIntEncodedWithTooBigInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(NativeUIntEncodedWithTooBigUInt64))));
        }

        [Fact]
        public void SupportsProperlyEncodedPointerConstants()
        {
            var source = typeof(ILMethodsWithDefaultParameters).GetMethod(nameof(PointersEncodedProperly));
            var shim = Shim.Create<Func<bool>>(source);
            Assert.True(shim());
        }

        [Fact]
        public void DoesNotSupportBadlyEncodedPointerConstants()
        {
            var type = typeof(ILMethodsWithDefaultParameters);
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(PointerEncodedWithBoolean))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(PointerEncodedWithChar))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(PointerEncodedWithTooSmallInt8))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(PointerEncodedWithTooSmallInt16))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(PointerEncodedWithTooSmallInt32))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(PointerEncodedWithTooSmallInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(PointerEncodedWithTooBigInt64))));
            Assert.Throws<InvalidProgramException>(() => Shim.Create<Func<bool>>(type.GetMethod(nameof(PointerEncodedWithTooBigUInt64))));
        }

        [Fact]
        public void SupportsDateTimeConstantAttribute()
        {
            var source = typeof(ILMethodsWithDefaultParameters).GetMethod(nameof(DateTimeEncodedAttribute));
            var shim = Shim.Create<Func<bool>>(source);
            Assert.True(shim());
        }
    }
}
