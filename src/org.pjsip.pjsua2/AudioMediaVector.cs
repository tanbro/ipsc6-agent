//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 4.0.2
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace org.pjsip.pjsua2 {

public class AudioMediaVector : global::System.IDisposable, global::System.Collections.IEnumerable, global::System.Collections.Generic.IList<AudioMedia>
 {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal AudioMediaVector(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(AudioMediaVector obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~AudioMediaVector() {
    Dispose(false);
  }

  public void Dispose() {
    Dispose(true);
    global::System.GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          pjsua2PINVOKE.delete_AudioMediaVector(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public AudioMediaVector(global::System.Collections.IEnumerable c) : this() {
    if (c == null)
      throw new global::System.ArgumentNullException("c");
    foreach (AudioMedia element in c) {
      this.Add(element);
    }
  }

  public AudioMediaVector(global::System.Collections.Generic.IEnumerable<AudioMedia> c) : this() {
    if (c == null)
      throw new global::System.ArgumentNullException("c");
    foreach (AudioMedia element in c) {
      this.Add(element);
    }
  }

  public bool IsFixedSize {
    get {
      return false;
    }
  }

  public bool IsReadOnly {
    get {
      return false;
    }
  }

  public AudioMedia this[int index]  {
    get {
      return getitem(index);
    }
    set {
      setitem(index, value);
    }
  }

  public int Capacity {
    get {
      return (int)capacity();
    }
    set {
      if (value < size())
        throw new global::System.ArgumentOutOfRangeException("Capacity");
      reserve((uint)value);
    }
  }

  public int Count {
    get {
      return (int)size();
    }
  }

  public bool IsSynchronized {
    get {
      return false;
    }
  }

  public void CopyTo(AudioMedia[] array)
  {
    CopyTo(0, array, 0, this.Count);
  }

  public void CopyTo(AudioMedia[] array, int arrayIndex)
  {
    CopyTo(0, array, arrayIndex, this.Count);
  }

  public void CopyTo(int index, AudioMedia[] array, int arrayIndex, int count)
  {
    if (array == null)
      throw new global::System.ArgumentNullException("array");
    if (index < 0)
      throw new global::System.ArgumentOutOfRangeException("index", "Value is less than zero");
    if (arrayIndex < 0)
      throw new global::System.ArgumentOutOfRangeException("arrayIndex", "Value is less than zero");
    if (count < 0)
      throw new global::System.ArgumentOutOfRangeException("count", "Value is less than zero");
    if (array.Rank > 1)
      throw new global::System.ArgumentException("Multi dimensional array.", "array");
    if (index+count > this.Count || arrayIndex+count > array.Length)
      throw new global::System.ArgumentException("Number of elements to copy is too large.");
    for (int i=0; i<count; i++)
      array.SetValue(getitemcopy(index+i), arrayIndex+i);
  }

  public AudioMedia[] ToArray() {
    AudioMedia[] array = new AudioMedia[this.Count];
    this.CopyTo(array);
    return array;
  }

  global::System.Collections.Generic.IEnumerator<AudioMedia> global::System.Collections.Generic.IEnumerable<AudioMedia>.GetEnumerator() {
    return new AudioMediaVectorEnumerator(this);
  }

  global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator() {
    return new AudioMediaVectorEnumerator(this);
  }

  public AudioMediaVectorEnumerator GetEnumerator() {
    return new AudioMediaVectorEnumerator(this);
  }

  // Type-safe enumerator
  /// Note that the IEnumerator documentation requires an InvalidOperationException to be thrown
  /// whenever the collection is modified. This has been done for changes in the size of the
  /// collection but not when one of the elements of the collection is modified as it is a bit
  /// tricky to detect unmanaged code that modifies the collection under our feet.
  public sealed class AudioMediaVectorEnumerator : global::System.Collections.IEnumerator
    , global::System.Collections.Generic.IEnumerator<AudioMedia>
  {
    private AudioMediaVector collectionRef;
    private int currentIndex;
    private object currentObject;
    private int currentSize;

    public AudioMediaVectorEnumerator(AudioMediaVector collection) {
      collectionRef = collection;
      currentIndex = -1;
      currentObject = null;
      currentSize = collectionRef.Count;
    }

    // Type-safe iterator Current
    public AudioMedia Current {
      get {
        if (currentIndex == -1)
          throw new global::System.InvalidOperationException("Enumeration not started.");
        if (currentIndex > currentSize - 1)
          throw new global::System.InvalidOperationException("Enumeration finished.");
        if (currentObject == null)
          throw new global::System.InvalidOperationException("Collection modified.");
        return (AudioMedia)currentObject;
      }
    }

    // Type-unsafe IEnumerator.Current
    object global::System.Collections.IEnumerator.Current {
      get {
        return Current;
      }
    }

    public bool MoveNext() {
      int size = collectionRef.Count;
      bool moveOkay = (currentIndex+1 < size) && (size == currentSize);
      if (moveOkay) {
        currentIndex++;
        currentObject = collectionRef[currentIndex];
      } else {
        currentObject = null;
      }
      return moveOkay;
    }

    public void Reset() {
      currentIndex = -1;
      currentObject = null;
      if (collectionRef.Count != currentSize) {
        throw new global::System.InvalidOperationException("Collection modified.");
      }
    }

    public void Dispose() {
        currentIndex = -1;
        currentObject = null;
    }
  }

  public void Clear() {
    pjsua2PINVOKE.AudioMediaVector_Clear(swigCPtr);
  }

  public void Add(AudioMedia x) {
    pjsua2PINVOKE.AudioMediaVector_Add(swigCPtr, AudioMedia.getCPtr(x));
  }

  private uint size() {
    uint ret = pjsua2PINVOKE.AudioMediaVector_size(swigCPtr);
    return ret;
  }

  private uint capacity() {
    uint ret = pjsua2PINVOKE.AudioMediaVector_capacity(swigCPtr);
    return ret;
  }

  private void reserve(uint n) {
    pjsua2PINVOKE.AudioMediaVector_reserve(swigCPtr, n);
  }

  public AudioMediaVector() : this(pjsua2PINVOKE.new_AudioMediaVector__SWIG_0(), true) {
  }

  public AudioMediaVector(AudioMediaVector other) : this(pjsua2PINVOKE.new_AudioMediaVector__SWIG_1(AudioMediaVector.getCPtr(other)), true) {
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public AudioMediaVector(int capacity) : this(pjsua2PINVOKE.new_AudioMediaVector__SWIG_2(capacity), true) {
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  private AudioMedia getitemcopy(int index) {
    global::System.IntPtr cPtr = pjsua2PINVOKE.AudioMediaVector_getitemcopy(swigCPtr, index);
    AudioMedia ret = (cPtr == global::System.IntPtr.Zero) ? null : new AudioMedia(cPtr, false);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  private AudioMedia getitem(int index) {
    global::System.IntPtr cPtr = pjsua2PINVOKE.AudioMediaVector_getitem(swigCPtr, index);
    AudioMedia ret = (cPtr == global::System.IntPtr.Zero) ? null : new AudioMedia(cPtr, false);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  private void setitem(int index, AudioMedia val) {
    pjsua2PINVOKE.AudioMediaVector_setitem(swigCPtr, index, AudioMedia.getCPtr(val));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void AddRange(AudioMediaVector values) {
    pjsua2PINVOKE.AudioMediaVector_AddRange(swigCPtr, AudioMediaVector.getCPtr(values));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public AudioMediaVector GetRange(int index, int count) {
    global::System.IntPtr cPtr = pjsua2PINVOKE.AudioMediaVector_GetRange(swigCPtr, index, count);
    AudioMediaVector ret = (cPtr == global::System.IntPtr.Zero) ? null : new AudioMediaVector(cPtr, true);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void Insert(int index, AudioMedia x) {
    pjsua2PINVOKE.AudioMediaVector_Insert(swigCPtr, index, AudioMedia.getCPtr(x));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void InsertRange(int index, AudioMediaVector values) {
    pjsua2PINVOKE.AudioMediaVector_InsertRange(swigCPtr, index, AudioMediaVector.getCPtr(values));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void RemoveAt(int index) {
    pjsua2PINVOKE.AudioMediaVector_RemoveAt(swigCPtr, index);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void RemoveRange(int index, int count) {
    pjsua2PINVOKE.AudioMediaVector_RemoveRange(swigCPtr, index, count);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public static AudioMediaVector Repeat(AudioMedia value, int count) {
    global::System.IntPtr cPtr = pjsua2PINVOKE.AudioMediaVector_Repeat(AudioMedia.getCPtr(value), count);
    AudioMediaVector ret = (cPtr == global::System.IntPtr.Zero) ? null : new AudioMediaVector(cPtr, true);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void Reverse() {
    pjsua2PINVOKE.AudioMediaVector_Reverse__SWIG_0(swigCPtr);
  }

  public void Reverse(int index, int count) {
    pjsua2PINVOKE.AudioMediaVector_Reverse__SWIG_1(swigCPtr, index, count);
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public void SetRange(int index, AudioMediaVector values) {
    pjsua2PINVOKE.AudioMediaVector_SetRange(swigCPtr, index, AudioMediaVector.getCPtr(values));
    if (pjsua2PINVOKE.SWIGPendingException.Pending) throw pjsua2PINVOKE.SWIGPendingException.Retrieve();
  }

  public bool Contains(AudioMedia value) {
    bool ret = pjsua2PINVOKE.AudioMediaVector_Contains(swigCPtr, AudioMedia.getCPtr(value));
    return ret;
  }

  public int IndexOf(AudioMedia value) {
    int ret = pjsua2PINVOKE.AudioMediaVector_IndexOf(swigCPtr, AudioMedia.getCPtr(value));
    return ret;
  }

  public int LastIndexOf(AudioMedia value) {
    int ret = pjsua2PINVOKE.AudioMediaVector_LastIndexOf(swigCPtr, AudioMedia.getCPtr(value));
    return ret;
  }

  public bool Remove(AudioMedia value) {
    bool ret = pjsua2PINVOKE.AudioMediaVector_Remove(swigCPtr, AudioMedia.getCPtr(value));
    return ret;
  }

}

}
