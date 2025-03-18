# Animal Crossing Style Camera System for Unity

This package provides a camera system that mimics the style of Animal Crossing: New Horizons using Cinemachine. The system includes:

1. Fixed isometric-style camera that follows the player
2. Automatic indoor/outdoor detection with different camera settings
3. Smooth transitions between camera states
4. Special area triggers for custom camera behavior
5. Subtle camera rotation adjustments based on player movement

## Setup Instructions

### Basic Setup

1. **Create a Camera Setup**:
   - Create an empty GameObject in your scene named "AC_CameraSystem"
   - Add the `AnimalCrossingCameraSystem` component to this GameObject
   - Assign your player character to the "Target" field

2. **Configure the Main Camera**:
   - Make sure your main camera has a `CinemachineBrain` component
   - Alternatively, the system will add one automatically if not found

3. **Configure Camera Settings**:
   - Adjust the "Outdoor Camera Settings" and "Indoor Camera Settings" to your preference
   - The default values are set to mimic Animal Crossing's camera style

### Advanced Features

1. **Add Camera Extensions**:
   - Add the `AnimalCrossingCameraExtensions` component to your player character
   - This enables subtle camera rotation based on player movement
   - Assign the main `AnimalCrossingCameraSystem` to the "Main Camera System" field

2. **Create Special Camera Areas**:
   - Create an empty GameObject in your scene
   - Add a Collider component (Box Collider, Sphere Collider, etc.) and ensure it's set as a trigger
   - Add the `AnimalCrossingCameraAreaSetup` component
   - Configure the area settings as needed
   - Make sure to set the GameObject's layer to one that's included in the "Special Area Layer" mask in the `AnimalCrossingCameraExtensions` component

3. **Indoor/Outdoor Detection**:
   - The system automatically detects indoor areas by raycasting upward to check for roofs
   - Create a layer for roof objects and assign it to the "Roof Layer" field in the `AnimalCrossingCameraSystem` component
   - Alternatively, use special camera areas to manually define indoor/outdoor regions

## Usage Tips

1. **Camera Angles**:
   - For the most authentic Animal Crossing look, use these settings:
     - Outdoor Pitch: 45 degrees
     - Outdoor Height: 10
     - Outdoor Distance: 10
     - Indoor Pitch: 40 degrees
     - Indoor Height: 7
     - Indoor Distance: 7

2. **Smooth Transitions**:
   - Adjust the "Transition Time" value to control how quickly the camera blends between states
   - Lower values (0.5-1.0) provide quick transitions similar to Animal Crossing
   - Higher values (1.5-3.0) provide more cinematic, gradual transitions

3. **Player Movement**:
   - The camera works best with a character controller that uses the camera's forward direction for movement
   - See the existing `PlayerController` for an example of camera-relative movement

4. **Special Areas**:
   - Use special camera areas for:
     - Forcing indoor camera settings in areas without roofs (like pergolas or open structures)
     - Creating unique camera angles for specific locations
     - Ensuring consistent camera behavior in complex environments

## Troubleshooting

1. **Camera Not Following Player**:
   - Ensure the player is assigned as the "Target" in the `AnimalCrossingCameraSystem`
   - Check that the player has the correct tag if using automatic player detection

2. **Indoor/Outdoor Detection Not Working**:
   - Verify that roof objects are assigned to the correct layer
   - Check the "Raycast Distance" value - it should be high enough to reach the roof
   - Use special camera areas as a fallback for problematic areas

3. **Camera Rotation Issues**:
   - If the camera rotation is too sensitive, reduce the "Max Rotation Adjustment" value
   - If rotation feels sluggish, increase the "Rotation Adjustment Speed" value

## Example Scene Setup

For a complete example of how to set up the Animal Crossing camera system:

1. Create a new scene with a player character and environment
2. Add the camera system as described above
3. Create a few special camera areas for testing
4. Ensure you have both indoor and outdoor areas to test the automatic detection

## Credits

This camera system was created to mimic the style of Animal Crossing: New Horizons, developed by Nintendo. The implementation uses Unity's Cinemachine package for smooth camera control and transitions. 