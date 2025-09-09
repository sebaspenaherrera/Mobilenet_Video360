# Mobilenet Video360

Mobilenet Video360 is a **Unity-based testbed** developed with **Unity 2022.3** for evaluating **360-video streaming services**.  
The project is designed to extract **Key Quality Indicators (KQIs)** that characterize the user experience of video playback.  

⚠️ **Note**: KQIs differ from KPIs.  
- **KQIs (Key Quality Indicators)** → End-user Quality of Experience (QoE) metrics.  
- **KPIs (Key Performance Indicators)** → Network-side performance metrics.  

---

## 📌 Features
- Unity 2022.3 project for immersive **360-video playback**.  
- Extraction of detailed **KQIs** (video, network, resolution, frame rate, media, and player state).  
- Demo/testbed to validate streaming quality in controlled scenarios.  

---

## 📂 Project Structure
- **`Assets/Scripts/KQI_group.cs`** → Main file where all KQIs are defined.  
- **Unity Scenes & Prefabs** → Manage playback and interaction.  
- **Networking/Video Components** → Integrates with playback service for data collection.  

---

## 📊 Key Quality Indicators (KQIs)

The following KQIs are tracked and exported from the player runtime:  

| **Category**     | **KQI**                  | **Type** | **Description** |
|------------------|---------------------------|----------|-----------------|
| **General**      | `timestamp`              | string   | Timestamp of measurement. |
|                  | `initTime`               | double   | Initialization time (s). |
|                  | `stallTime`              | double   | Current stall time (s). |
|                  | `overallStallTime`       | double   | Total accumulated stall time (s). |
|                  | `bufferTime`             | double   | Buffering time (s). |
| **Network**      | `rtt_ping`               | double   | Round-trip time (ping) in ms. |
|                  | `rtt`                    | double   | Measured round-trip time in ms. |
|                  | `estimatedBWExoplayer`   | double   | Estimated bandwidth from ExoPlayer. |
|                  | `tx_rate`                | double   | Transmission rate (bps). |
|                  | `rx_rate`                | double   | Reception rate (bps). |
|                  | `tx_packetRate`          | long     | Transmission packet rate (pps). |
|                  | `rx_packetRate`          | long     | Reception packet rate (pps). |
| **Resolution**   | `width`                  | int      | Video width (px). |
|                  | `height`                 | int      | Video height (px). |
|                  | `resolution`             | string   | Resolution as string (e.g., `1920x1080`). |
|                  | `res_switches`           | int      | Number of resolution switches. |
|                  | `res_profile`            | int      | Current resolution profile ID. |
| **Frame Rate**   | `displayed_frameRate`    | double   | Displayed frame rate (fps). |
|                  | `encoded_frameRate`      | double   | Encoded frame rate (fps). |
|                  | `screen_frameRate`       | double   | Screen refresh frame rate (fps). |
|                  | `min_screen_frameRate`   | double   | Minimum observed screen frame rate (fps). |
|                  | `max_screen_frameRate`   | double   | Maximum observed screen frame rate (fps). |
| **Frame Info**   | `duplicatedFrames`       | int      | Count of duplicated frames. |
|                  | `perfectFrames`          | float    | Count of perfect frames rendered. |
|                  | `skippedFrames`          | int      | Number of skipped frames. |
|                  | `unityDroppedFrames`     | int      | Dropped frames inside Unity. |
| **Media Info**   | `playerDescription`      | string   | Player description string. |
|                  | `maxFrameNumber`         | int      | Maximum frame index. |
|                  | `durationFrames`         | int      | Total number of frames. |
|                  | `durationMedia`          | double   | Media duration (s). |
| **Player State** | `hasAudio`               | bool     | Indicates if audio is present. |
|                  | `hasVideo`               | bool     | Indicates if video is present. |
|                  | `isStalled`              | bool     | Indicates if player is stalled. |
|                  | `isPlaying`              | bool     | Indicates if player is playing. |
|                  | `isBuffering`            | bool     | Indicates if player is buffering. |

---

**Note 1:** You can get some KPIs by using the transport network API. If you are using Amari-based software, you may be interested in: [API Amari](https://github.com/sebaspenaherrera/api-amari).
**Note 2:** Additionally, you can gather some metrics from the CPE. To do so, you need to configure the credentials and make sure the API procedure is similar. This has been tested with Huawei CPE Pro 2.

## 🚀 Getting Started

### Prerequisites
- [Unity Hub](https://unity.com/download)  
- Unity **2022.3 LTS** installed.  

### Installation
1. Clone the repository:  
   ```bash
   git clone https://github.com/sebaspenaherrera/Mobilenet_Video360.git

2. Open the project in Unity 2022.3.

3. Load the scenes provided in the project.

## 📌 Usage

- Run the demo scene in the Unity editor or build for your target device. By default, Meta Quest 2. If the device is different, recompile the code following the providers' suggestions
- Configure the App
  * Point to the right API for data collection
  * Select the video
  * Enable/Disable the CPE metric with the toggle.
    +  **Note**: This will enable a new menu screen to configure CPE's API endpoint and credentials. 
- Play a 360-video stream and gather some interesting metrics!

- **Optional**: Monitor and log KQIs for performance evaluation. By default, you can use [360-video WebUI](https://github.com/sebaspenaherrera/Video360_WebUI).
