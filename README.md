# Input-System-Extension

A flexible and scalable input framework for Unity built on top of the **New Input System**, designed to simplify input configuration, rebinding, visualization, and runtime handling across multiple devices.

---

## 📦 Download

[Input System Extension - Package v0.0.4](https://drive.google.com/file/d/1cGOsVRLhczhDOlkHYGF7uKJJ5M77i_SJ/view?usp=drive_link)
/
[Documentation](https://drive.google.com/file/d/1QG0QRXzMXqneKkG6jtITSs9xk-9G7YB3/view?usp=drive_link)

---

## 🧠 Overview

The **Input System Extension** provides a complete solution for managing player input in Unity, including:

* Runtime rebinding (keyboard, mouse, gamepad, touch)
* Input visualization (text + dynamic icons)
* Persistent binding storage
* Event-driven input handling
* Editor tools for fast UI setup

It is designed to work seamlessly with **TextMeshPro**, providing a clean and customizable UI workflow.

---

## ✨ Features

### 🎮 Runtime Rebinding System

* Full rebind lifecycle:

  * Start / Complete / Cancel
  * Duplicate prevention
  * Composite bindings support
* Real-time UI updates
* Works with both standard UI and TMP

---

### 💾 Persistent Input Saving

* Automatic save/load of binding overrides
* Uses PlayerPrefs as backend
* No manual serialization required

---

### 🖼️ Input Display (Text + Icons)

* Automatic switching between:

  * Keyboard text
  * Gamepad icons (Xbox / PlayStation)
* Fallback to text when no icon is available
* Fully customizable icon mappings

---

### ⚡ Event-Based Input System

* Convert raw input into structured events:

  * Pressed
  * Hold
  * Released
* Supports multiple listeners per input
* Owner-based system prevents conflicts
* Works with any input type (float, Vector2, bool, etc.)

---

### 🔍 Input Detection Utilities

* Detect activity from:

  * Keyboard
  * Mouse
  * Gamepad
  * Touch
* Useful for:

  * Switching UI prompts
  * Detecting active device

---

### 🧰 Editor & Workflow Tools

* Built-in Project Settings panel
* Automatic asset creation
* Prefab instantiation tools
* Ready-to-use UI components

---

### 🔄 Reset System

* Reset individual bindings
* Reset all bindings at once
* Ideal for settings menus

---

## 🧱 Architecture

The system is built around a few core components:

* **Rebind Control Managers**
  Handle UI, rebinding logic, and binding display.

* **Input System Event Dispatcher**
  Centralized event system for handling input states.

* **InputSystemExtensionData (ScriptableObject)**
  Stores:

  * Default input asset
  * Icon mappings
  * PlayerPrefs key

* **Utility Systems**

  * Input detection helpers
  * Binding save/load system

---

## ▶️ Getting Started

1. Import the package into your Unity project
2. Ensure the **New Input System** is enabled
3. Create (or auto-generate) the `Input System Extension Data` asset
4. Add UI prefabs via:

   ```
   GameObject → Tools → Input System Extension
   ```
5. Configure your input actions and bindings
6. (Optional) Use the event system for gameplay logic

---

## 🎥 Tutorials

For usage tutorials and demonstrations, check the YouTube playlist:

👉 [Watch Here](https://www.youtube.com/playlist?list=PL5hnfx09yM4Kqkhx0KHyUW0kWviPMTPCs)

---

## 💬 Feedback

Your feedback is invaluable!
If you encounter bugs or have suggestions, feel free to open an issue or reach out.

---

## 📁 Legacy Versions

[Old Versions - Package](https://drive.google.com/drive/folders/1882_aAK2gTwdIFMDfoZKAeK-3r-N3Z_2?usp=drive_link)

---

## 📌 Current Version

**Git Version:** v0.4