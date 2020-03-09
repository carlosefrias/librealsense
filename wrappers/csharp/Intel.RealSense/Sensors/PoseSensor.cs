// License: Apache 2.0. See LICENSE file in root directory.
// Copyright(c) 2017 Intel Corporation. All Rights Reserved.

namespace Intel.RealSense
{
    using System;
    using System.Runtime.InteropServices;

    public class PoseSensor : Sensor
    {
        internal PoseSensor(IntPtr ptr) : base(ptr)
        { }

        public static PoseSensor FromSensor(Sensor sensor)
        {
            if (!sensor.Is(Extension.PoseSensor))
            {
                throw new ArgumentException($"Sensor does not support {nameof(Extension.PoseSensor)}");
            }
            return Create<PoseSensor>(sensor.Handle);
        }

        public byte[] ExportLocalizationMap()
        {
            object error;
            var rawDataBuffer = NativeMethods.rs2_export_localization_map(Handle, out error);

            var start = NativeMethods.rs2_get_raw_data(rawDataBuffer, out error);
            var size = NativeMethods.rs2_get_raw_data_size(rawDataBuffer, out error);

            var managedBytes = new byte[size];
            Marshal.Copy(start, managedBytes, 0, size);
            NativeMethods.rs2_delete_raw_data(rawDataBuffer);

            return managedBytes;
        }

        public bool ImportLocalizationMap(byte[] mapBytes)
        {
            var nativeBytes = IntPtr.Zero;
            try
            {
                nativeBytes = Marshal.AllocHGlobal(mapBytes.Length);
                Marshal.Copy(mapBytes, 0, nativeBytes, mapBytes.Length);
                object error;
                var res = NativeMethods.rs2_import_localization_map(Handle, nativeBytes, (uint)mapBytes.Length, out error);
                return res != 0;
            }
            finally
            {
                Marshal.FreeHGlobal(nativeBytes);
            }
        }

        public bool SetStaticNode(string guid, Math.Vector position, Math.Quaternion rotation)
        {
            object error;
            var res = NativeMethods.rs2_set_static_node(Handle, guid, position, rotation, out error);
            return res != 0;
        }

        public bool GetStaticNode(string guid, out Math.Vector position, out Math.Quaternion rotation)
        {
            object error;
            var res = NativeMethods.rs2_get_static_node(Handle, guid, out position, out rotation, out error);
            return res != 0;
        }

        public void DisableReLocalization()
        {
            object outObj;
            NativeMethods.rs2_set_option(Handle, Option.EnableRelocalization, 0.0f, out outObj);
        }

        public void EnableReLocalization()
        {
            object outObj;
            NativeMethods.rs2_set_option(Handle, Option.EnableRelocalization, 1.0f, out outObj);
        }

        public float GetRelocalizationValue()
        {
            object outObj;
            return NativeMethods.rs2_get_option(Handle, Option.EnableRelocalization, out outObj);
        }

        public float GetMotionModuleTemperature()
        {
            object outObj;
            return NativeMethods.rs2_get_option(Handle, Option.MotionModuleTemperature, out outObj);
        }

        public OptionsList GetOptionsList()
        {
            object errorObj;
            var ptr = NativeMethods.rs2_get_options_list(Handle, out errorObj);
            return new OptionsList(ptr);
        }
    }
}
