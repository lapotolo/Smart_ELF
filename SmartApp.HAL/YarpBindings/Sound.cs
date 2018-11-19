//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 3.0.12
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------


public class Sound : Portable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;

  internal Sound(global::System.IntPtr cPtr, bool cMemoryOwn) : base(yarpPINVOKE.Sound_SWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(Sound obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~Sound() {
    Dispose();
  }

  public override void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          yarpPINVOKE.delete_Sound(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
      base.Dispose();
    }
  }

  public Sound(int bytesPerSample) : this(yarpPINVOKE.new_Sound__SWIG_0(bytesPerSample), true) {
  }

  public Sound() : this(yarpPINVOKE.new_Sound__SWIG_1(), true) {
  }

  public Sound(Sound alt) : this(yarpPINVOKE.new_Sound__SWIG_2(Sound.getCPtr(alt)), true) {
    if (yarpPINVOKE.SWIGPendingException.Pending) throw yarpPINVOKE.SWIGPendingException.Retrieve();
  }

  public Sound subSound(uint first_sample, uint last_sample) {
    Sound ret = new Sound(yarpPINVOKE.Sound_subSound(swigCPtr, first_sample, last_sample), true);
    return ret;
  }

  public void resize(uint samples, uint channels) {
    yarpPINVOKE.Sound_resize__SWIG_0(swigCPtr, samples, channels);
  }

  public void resize(uint samples) {
    yarpPINVOKE.Sound_resize__SWIG_1(swigCPtr, samples);
  }

  public int get(uint sample, uint channel) {
    int ret = yarpPINVOKE.Sound_get__SWIG_0(swigCPtr, sample, channel);
    return ret;
  }

  public int get(uint sample) {
    int ret = yarpPINVOKE.Sound_get__SWIG_1(swigCPtr, sample);
    return ret;
  }

  public void set(int value, uint sample, uint channel) {
    yarpPINVOKE.Sound_set__SWIG_0(swigCPtr, value, sample, channel);
  }

  public void set(int value, uint sample) {
    yarpPINVOKE.Sound_set__SWIG_1(swigCPtr, value, sample);
  }

  public int getSafe(uint sample, uint channel) {
    int ret = yarpPINVOKE.Sound_getSafe__SWIG_0(swigCPtr, sample, channel);
    return ret;
  }

  public int getSafe(uint sample) {
    int ret = yarpPINVOKE.Sound_getSafe__SWIG_1(swigCPtr, sample);
    return ret;
  }

  public void setSafe(int value, uint sample, uint channel) {
    yarpPINVOKE.Sound_setSafe__SWIG_0(swigCPtr, value, sample, channel);
  }

  public void setSafe(int value, uint sample) {
    yarpPINVOKE.Sound_setSafe__SWIG_1(swigCPtr, value, sample);
  }

  public bool isSample(uint sample, uint channel) {
    bool ret = yarpPINVOKE.Sound_isSample__SWIG_0(swigCPtr, sample, channel);
    return ret;
  }

  public bool isSample(uint sample) {
    bool ret = yarpPINVOKE.Sound_isSample__SWIG_1(swigCPtr, sample);
    return ret;
  }

  public void clear() {
    yarpPINVOKE.Sound_clear(swigCPtr);
  }

  public uint getFrequency() {
    uint ret = yarpPINVOKE.Sound_getFrequency(swigCPtr);
    return ret;
  }

  public void setFrequency(uint freq) {
    yarpPINVOKE.Sound_setFrequency(swigCPtr, freq);
  }

  public uint getBytesPerSample() {
    uint ret = yarpPINVOKE.Sound_getBytesPerSample(swigCPtr);
    return ret;
  }

  public uint getSamples() {
    uint ret = yarpPINVOKE.Sound_getSamples(swigCPtr);
    return ret;
  }

  public uint getChannels() {
    uint ret = yarpPINVOKE.Sound_getChannels(swigCPtr);
    return ret;
  }

  public new bool read(ConnectionReader connection) {
    bool ret = yarpPINVOKE.Sound_read(swigCPtr, ConnectionReader.getCPtr(connection));
    if (yarpPINVOKE.SWIGPendingException.Pending) throw yarpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public new bool write(ConnectionWriter connection) {
    bool ret = yarpPINVOKE.Sound_write(swigCPtr, ConnectionWriter.getCPtr(connection));
    if (yarpPINVOKE.SWIGPendingException.Pending) throw yarpPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public System.IntPtr getRawData() { return yarpPINVOKE.Sound_getRawData(swigCPtr); }

  public uint getRawDataSize() {
    uint ret = yarpPINVOKE.Sound_getRawDataSize(swigCPtr);
    return ret;
  }

}