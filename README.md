<div>
    <img src="https://readme-typing-svg.demolab.com/?pause=1&size=50&color=f75c7e&center=True&width=1200&height=120&vCenter=True&lines=Click+the+⭐+Star+please .;Any+questions+can+be+asked+in+Issue." />
</div>

# Project Description
*StreamCast* is a real-time Camera and Screen Sharing solution developed in .NET MVC. Utilizing WebSocket for real-time communication, FFmpeg for media capture and encoding, and Nginx as the media server, StreamCast allows users to share their camera feed and screen with others seamlessly and securely.

# Key Technologies

WebSocket: Ensures a persistent, low-latency connection between clients and the server, making it ideal for real-time communication required in video and screen-sharing applications.

FFmpeg: A powerful multimedia framework that captures, encodes, and transcodes video and audio streams in various formats. In this project, FFmpeg is used for capturing the camera and screen feeds, encoding them into a streamable format.

Nginx: Acts as the media server in this architecture. With its RTMP (Real-Time Messaging Protocol) module, Nginx is configured to handle incoming video streams and distribute them to clients efficiently.

# Project Features

Camera Feed Streaming: Users can stream video from their webcam to viewers in real-time.

Screen Sharing: Enables users to share their screen with others, providing a seamless way to present information or collaborate remotely.

Low Latency Streaming: With the use of WebSocket and Nginx’s optimized media handling, the project delivers low-latency streaming to ensure minimal delay between the source and viewers.

Scalable and High-Performance: Leveraging Nginx for media distribution allows the application to handle a large number of simultaneous viewers without compromising on performance.

Cross-Platform Support: Designed to work across various devices and browsers, ensuring accessibility for all users.

# Technical Workflow

Capture & Encoding: The video and screen feeds are captured through FFmpeg, which encodes them into a suitable format for streaming.

WebSocket Communication: The WebSocket connection handles control and signaling data, ensuring smooth streaming initiation and maintenance.

Nginx Streaming: The encoded stream is pushed to the Nginx server, which then distributes it to connected viewers in real-time.

# Use Cases

Remote Collaboration: Ideal for remote work, allowing users to share screens and video during meetings and collaborative sessions.
Online Presentations: Can be used in online presentations, webinars, and training sessions where real-time screen sharing and camera feeds are essential.
Live Demonstrations: Useful for live demonstrations where visual instructions are required, such as software demos or technical troubleshooting.

StreamCast represents a robust, high-quality streaming solution, delivering smooth, real-time video and screen sharing capabilities for modern web applications.

# Out Put
![StreamCast](https://github.com/user-attachments/assets/8874044d-517b-45b1-89eb-943a8caf3dee)


