//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 3.0.12
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------


public class DriverCreator : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal DriverCreator(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(DriverCreator obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~DriverCreator() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          yarpPINVOKE.delete_DriverCreator(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public new string toString_c() {
    string ret = yarpPINVOKE.DriverCreator_toString_c(swigCPtr);
    return ret;
  }

  public virtual DeviceDriver create() {
    global::System.IntPtr cPtr = yarpPINVOKE.DriverCreator_create(swigCPtr);
    DeviceDriver ret = (cPtr == global::System.IntPtr.Zero) ? null : new DeviceDriver(cPtr, false);
    return ret;
  }

  public virtual string getName() {
    string ret = yarpPINVOKE.DriverCreator_getName(swigCPtr);
    return ret;
  }

  public virtual string getWrapper() {
    string ret = yarpPINVOKE.DriverCreator_getWrapper(swigCPtr);
    return ret;
  }

  public virtual string getCode() {
    string ret = yarpPINVOKE.DriverCreator_getCode(swigCPtr);
    return ret;
  }

  public virtual PolyDriver owner() {
    global::System.IntPtr cPtr = yarpPINVOKE.DriverCreator_owner(swigCPtr);
    PolyDriver ret = (cPtr == global::System.IntPtr.Zero) ? null : new PolyDriver(cPtr, false);
    return ret;
  }

}