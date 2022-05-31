# ServerTesting
 Simple project for testing the server application

See the [ItemSpawner](Assets/ItemSpawner.cs) script for sample networking functionality.

If testing with a running backend system, ensure:
1. IP has been set correctly (localhost if testing on windows / in editor, local IPv4 if on the same network)
2. Build target in the server matches (build this app for android => build assetBundles for android, test in editor => build for windows/unix)
3. Click the "Load Asset" button before "Spawn Asset"
