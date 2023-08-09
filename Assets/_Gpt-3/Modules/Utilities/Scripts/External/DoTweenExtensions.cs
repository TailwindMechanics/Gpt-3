using DG.Tweening;
using System;
using UniRx;


namespace Modules.Utilities.External
{
	public static class DoTweenExtensions
	{
		public static IObservable<Tween> ToObservable(this Tween tween, bool emmitOnKill=true)
		{
			tween.Pause();
			return Observable.Create<Tween>((observer) =>
			{
				if (tween.IsComplete())
				{
					observer.OnError(new Exception("Already completed tween"));
					observer.OnCompleted();
				}

				if (!tween.IsPlaying())
				{
					tween.Play();
				}

				tween.onComplete += () =>
				{
					observer.OnNext(tween);
					observer.OnCompleted();
				};

				tween.onKill += () =>
				{
					if (emmitOnKill)
					{
						observer.OnNext(tween);
					}

					observer.OnCompleted();
				};

				return Disposable.Create(() =>
				{
					if (!tween.IsComplete())
					{
						tween.Kill();
					}
				});
			});
		}
	}
}