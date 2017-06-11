using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LerpableParam
{
	public float start, target, curValue, progress, invDuration;
}

public class AnimationHandler : MonoBehaviour
{
	public Transform self;
	public CanvasGroup selfGroup;
	public AnimationCurve smoothCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
	float Smooth(float a) { return smoothCurve.Evaluate(a); }

	LerpableParam alpha, scale, altitude;

	void Awake()
	{
		alpha = new LerpableParam();
		scale = new LerpableParam();
		altitude = new LerpableParam();

		alpha.progress = 2f;
		scale.progress = 2f;
		altitude.progress = 2f;
	}

	void Update()
	{
		if (UpdateParam(alpha)) selfGroup.alpha = alpha.curValue;
		if (UpdateParam(scale)) self.localScale = Vector3.one * scale.curValue;
		if (UpdateParam(altitude)) self.localPosition = Vector3.up * altitude.curValue;
	}

	bool UpdateParam(LerpableParam par)
	{
		if (par.progress > 1) return false;

		par.progress += Time.deltaTime * par.invDuration;
		par.curValue = Mathf.Lerp(par.start, par.target, Smooth(par.progress));
		return true;
	}

	public void AlphaTo(float target, float duration)
	{
		if (duration == 0)
		{
			selfGroup.alpha = target;
			alpha.progress = 2;
			return;
		}

		alpha.start = selfGroup.alpha;
		alpha.target = target;
		alpha.progress = 0.0f;
		alpha.invDuration = 1.0f / duration;
	}

	public void ScaleTo(float target, float duration)
	{
		if (duration == 0)
		{
			self.localScale = Vector3.one * target;
			scale.progress = 2;
			return;
		}

		scale.start = self.localScale.x;
		scale.target = target;
		scale.progress = 0;
		scale.invDuration = 1 / duration;
	}

	public void AltitudeTo(float target, float duration)
	{
		if (duration == 0)
		{
			self.localPosition = Vector3.up * target;
			altitude.progress = 2;
			return;
		}

		altitude.start = self.localPosition.y;
		altitude.target = target;
		altitude.progress = 0;
		altitude.invDuration = 1 / duration;
	}
}
