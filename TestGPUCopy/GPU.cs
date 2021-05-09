using Cloo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGPUCopy
{
    class GPU
    {
        private ComputeCommandQueue queue;
        private ComputeContext context;
        private ComputeKernel kernel;

        private string ArrayCopy
        {
            get
            {
                return @"
                        kernel void ArrayCopy(global uchar* bufferIn, int inOffset, global uchar* bufferOut, int outOffset) 
                        {
                              int index = get_global_id(0);
                              bufferOut[index + outOffset] = bufferIn[index + inOffset];
                        }";
            }
        }

        public GPU()
        {
            context = new ComputeContext(ComputeDeviceTypes.Gpu, new ComputeContextPropertyList(ComputePlatform.Platforms[0]), null, IntPtr.Zero);
            queue = new ComputeCommandQueue(context, context.Devices[0], ComputeCommandQueueFlags.None);
            var program = new ComputeProgram(context, ArrayCopy);
            program.Build(null, null, null, IntPtr.Zero);

            kernel = program.CreateKernel("ArrayCopy");
        }

        public void GPUCopy(byte[] origin, int originOffset, byte[] dest, int destOffset, int lenght)
        {

            ComputeBuffer<byte> originBuffer, destBuffer;

            originBuffer = new ComputeBuffer<byte>(context,
                ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, origin);
            destBuffer = new ComputeBuffer<byte>(context,
                ComputeMemoryFlags.ReadOnly | ComputeMemoryFlags.UseHostPointer, dest);

            kernel.SetMemoryArgument(0, originBuffer);
            kernel.SetValueArgument(1, originOffset);
            kernel.SetMemoryArgument(2, destBuffer);
            kernel.SetValueArgument(3, destOffset);

            queue.Execute(kernel, new long[] { 0 }, new long[] { lenght }, null, new ComputeEventList());
            queue.Finish();
        }
    }
}
