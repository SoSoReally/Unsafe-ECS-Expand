using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

public  static partial class UnsafeCodeECS
{

    #region ECS
    public unsafe static Span<T> AsSpan<T>(this DynamicBuffer<T> dynamicBuffer) where T : unmanaged
    {
        return new Span<T>(dynamicBuffer.GetUnsafePtr(), dynamicBuffer.Length);
    }

    public unsafe static ReadOnlySpan<T> AsReadOnlySpan<T>(this DynamicBuffer<T> dynamicBuffer) where T : unmanaged
    {
        return new ReadOnlySpan<T>(dynamicBuffer.GetUnsafeReadOnlyPtr(), dynamicBuffer.Length);
    }

    public unsafe static void UnSafeAddSpan<T>(this ref DynamicBuffer<T> dynamicBuffer, Span<T> list) where T : unmanaged
    {
        if (list.Length <= 0) { return; }
        int elemSize = UnsafeUtility.SizeOf<T>();
        int oldLength = dynamicBuffer.Length;
        dynamicBuffer.ResizeUninitialized(oldLength + list.Length);

        byte* basePtr = (byte*)dynamicBuffer.GetUnsafePtr();
        UnsafeUtility.MemCpy(basePtr + (long)oldLength * elemSize, UnsafeUtility.AddressOf(ref list[0]), (long)elemSize * list.Length);
    }
    public unsafe static void AddRange<T>(this ref DynamicBuffer<T> dynamicBuffer, ReadOnlySpan<T> list) where T : unmanaged
    {
        if (list.Length <= 0) { return; }
        int elemSize = UnsafeUtility.SizeOf<T>();
        int oldLength = dynamicBuffer.Length;
        dynamicBuffer.ResizeUninitialized(oldLength + list.Length);

        byte* basePtr = (byte*)dynamicBuffer.GetUnsafePtr();
        fixed (void* ptr = &list.GetPinnableReference())
        {
            UnsafeUtility.MemCpy(basePtr + (long)oldLength * elemSize, ptr, (long)elemSize * list.Length);
        }
    }
    #region RefRO RefRW
    public unsafe static ref readonly T AsReadOnly<T>(this in ComponentLookup<T> componentLookup, in Entity entity) where T : unmanaged, IComponentData
    {
        return ref componentLookup.GetRefRO(entity).ValueRO;
    }

    public unsafe static ref T AsRef<T>(this in ComponentLookup<T> componentLookup, in Entity entity) where T : unmanaged, IComponentData
    {
        return ref componentLookup.GetRefRW(entity, false).ValueRW;
    }
    public unsafe static ref readonly T AsRWReadOnly<T>(this in ComponentLookup<T> componentLookup, in Entity entity) where T : unmanaged, IComponentData
    {
        return ref componentLookup.GetRefRW(entity, true).ValueRO;
    }

    #endregion
    #endregion


}
