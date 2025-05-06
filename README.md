# Cerumen Cleaning Simulation

A virtual reality simulation with a patient’s ear canal for students to practice cerumen removal effectively.

## How to Setup
### Prerequisites
- Need: VR headset, Haply device, VR-ready computer/laptop
- Install Unity Hub and Unity version 6000.0.40f1
- Install the Meta Quest Link app and follow the Quest setup instructions
- Clone the main branch of this repository

#### Unity setup
- In the Unity Hub, press Open > From Disk
- Select the AudiologyUnityProject folder from the repository
- Open the project to load dependencies

#### Quest
- Enable Developer Mode
- Connect to the Meta quest link app on the computer

#### Haply
- Follow Haply quick start guide
- Configure Haply as needed

### Running the project
- Make sure Haply and VR are connected to the computer
- In Unity, select the Intro Scene
- Press the play button

## Changelog
### Version 0.8.0
- Finished up scene layout (timer, buttons, XR rig)
- Polished UI with VR capabilities, completed switch to world space UI
- Started cerumen functionality in spawn_test branch
- Fixed Haply device and started testing physics and interactibility
- Code cleanup
### Version 0.6.0
- Added settings UI and refined previous UI
- Started implementation of world space UI for VR functionality
- File I/O with user's stats being saved after completing the simulation scene
- Most updates are in the develop branch + head_models branch
- Because of Haply limitations (broken part), we are unable to work with the device to have it connected with the curette model

### Version  0.4.0
- Added VR Functionality
- Implemented blender models for head and curette
- Created and tested new Haply script for cube feedback
- Started UI work in the UI additions branch (UI for moving between scenes, timer, progression bar)

### Version 0.2.0
- Initial setup for Github (Repository, LFS)
- Implemented and toyed with Haply samples in the Haply testing branch
- Laid out assets directory and created initial scenes
- Uploaded image asset and started UI for title screen
