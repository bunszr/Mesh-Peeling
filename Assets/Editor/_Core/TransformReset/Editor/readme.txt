Hi, thank you for getting this package

I made this when learning about custom editors and it has been quite useful since then. So i hope it will be useful for you too.

The tool is pretty straight foward, instead of selecting reset from the gear icon in the Transform component, just press any of the buttons and the parameter (position, rotation or cale) will reset itsel. 

You can also set a custom origin so that, when working with larger scenes, you can point the origin to whererever is more confortable to you.

best regards and good luck with your projects.


From version 1.2, there will be cases where resetting rotation may result in values like (180,180,180) or (0,-360,360) instead of (0,0,0).
I imagine that some math or parsing issue happens when passing from Euler Angles, to Quaternion and then to Euler again (to show the values in the tranform inspector).

Aside from looking weird, this shouldn't affect anything as internally the rotation is registered as (0,0,0) 
(can be tested using Debug.Log(transform.localEulerAngles)).