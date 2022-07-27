ModuleAnimateGenericEffects

This is an expansion of the stock  ModuleAnimateGeneric module.  It adds the following:

	1. Associate effects with the moving animation
	2. Modules now follow part symmetry
	3. animSpeed is now working to set the animation speed

Usage

This works by utilizing the stock EFFECTS stanzas as used in many parts.  See the 
stock documentation for information on how to set up the EFFECTS stanzas

The syntax for the module itself is identical to the stock ModuleAnimateGeneric, with 
the following additions:

		deployEffectName = deploy
		postDeployEffectName = deployed
		postDeployEffectLength = 0.5
		retractEffectName = retract
		postRetractEffectName = retracted
		postRetractEffectLength = 0.5
		animSpeed = 0.1
		animationRampSpeed = 0.005
		customAnimationSpeed = 1
		reverseGUIName = Play Reverse

where

	deployEffectName is the effect(s) which should be played when deploying
	postDeployEffectName is the effect(s) which should be played when deployment is completed.
	postDeployEffectLength is how long the post-deployment effect should be played
	retractEffectName is the effect(s) which should be played when retracting
	postRetractEffectName is the effect(s) which should be played when retractment is completed
	postRetractEffectLength is how long the post-retractment effect should be played
	animationRampSpeed controls how quickly the animation gets up to speed.  If not specified, starts at full speed
	customAnimationSpeed is a multiplier to adust the speed without having to change the model
	reverseGUIName is the text to put into the PAW to reverse the motion.  If not specified, then will not be active

Default Values

		deployEffectName = deploy
		postDeployEffectName = 
		postDeployEffectLength = 0.5
		retractEffectName = retract
		postRetractEffectName = 
		postRetractEffectLength = 0.5
		animSpeed = 1
		animationRampSpeed = 0.005
		customAnimationSpeed = 1
		reverseGUIName = 


	If no value is specified, no effect will be played