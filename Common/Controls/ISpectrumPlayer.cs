using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Controls
{
    public interface ISpectrumPlayer
    {
        int ActiveStreamHandle { get; }
        int GetFFTData(float[] fftDataBuffer);
        int GetFFTFrequencyIndex(int frequency);
    }
}
