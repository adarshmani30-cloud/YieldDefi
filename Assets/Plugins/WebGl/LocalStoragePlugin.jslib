mergeInto(LibraryManager.library, {
  SaveData: function (keyPtr, valuePtr) {
    var key = UTF8ToString(keyPtr);
    var value = UTF8ToString(valuePtr);
    localStorage.setItem(key, value);
  },

  LoadData: function (keyPtr) {
    var key = UTF8ToString(keyPtr);
    var value = localStorage.getItem(key);
    if (!value) return 0;

    var lengthBytes = lengthBytesUTF8(value) + 1;
    var stringOnWasmHeap = _malloc(lengthBytes);
    stringToUTF8(value, stringOnWasmHeap, lengthBytes);
    return stringOnWasmHeap;
  }
});