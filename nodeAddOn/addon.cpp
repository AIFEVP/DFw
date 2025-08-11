#include <napi.h>

// This is the C++ function that will be called from Node.js
Napi::Value Add(const Napi::CallbackInfo& info) {
    Napi::Env env = info.Env();

    // Check if the number of arguments is 2
    if (info.Length() < 2) {
        Napi::TypeError::New(env, "Wrong number of arguments").ThrowAsJavaScriptException();
        return env.Null();
    }

    // Check if the arguments are numbers
    if (!info[0].IsNumber() || !info[1].IsNumber()) {
        Napi::TypeError::New(env, "Wrong arguments").ThrowAsJavaScriptException();
        return env.Null();
    }

    // Extract the numbers from the JavaScript arguments
    double arg0 = info[0].As<Napi::Number>().DoubleValue();
    double arg1 = info[1].As<Napi::Number>().DoubleValue();

    // Perform the addition
    double sum = arg0 + arg1;

    // Return the result as a JavaScript number
    return Napi::Number::New(env, sum);
}

// This function registers our addon with Node.js
Napi::Object Init(Napi::Env env, Napi::Object exports) {
    // Expose the 'add' C++ function to JavaScript
    exports.Set(Napi::String::New(env, "add"),
                Napi::Function::New(env, Add));
    return exports;
}

// Register the addon module
NODE_API_MODULE(NODE_GYP_MODULE_NAME, Init)