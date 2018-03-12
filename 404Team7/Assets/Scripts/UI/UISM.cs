using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State {In_Game, Loading, Main, Level_Select, Paused, Settings, About}
public delegate void DeadResetHandler ();



public interface UISM {

	/// <summary>
	/// Go back up to parent menu if in menu. Or pause / resume game.
	/// Does nothing at main menu.
	/// </summary>
	void GoBack ();

	/// <summary>
	/// Sets the menu to a page corresponding to the specified state.
	/// </summary>
	/// <param name="state">Current state of game.</param>
	void SetPanel(string state);

	/// <summary>
	/// Shows the dead screen. You can provide a DeadResetHandler that is called
	/// when dead screen is covering the game, or null.
	/// </summary>
	/// <param name="drh">A DeadResetHandler function</param>
	void ShowDead(DeadResetHandler drh);

	/// <summary>
	/// Shows the hurt screen. Please provide alpha.
	/// </summary>
	/// <param name="alpha">Alpha value.</param>
	void ShowHurt (float alpha);

	/// <summary>
	/// Loads a specified scene. Scene ID can be found under bulid settings.
    /// File --> Build Settings
	/// </summary>
	/// <param name="which">Scene ID.</param>
	void LoadScene (int which);

	/// <summary>
	/// Clears all UI elements from screen.
	/// </summary>
	void ClearAll ();

	/// <summary>
	/// Pauses the game.
	/// </summary>
	void PauseGame ();

	/// <summary>
	/// Resumes the game.
	/// </summary>
	void ResumeGame ();

	/// <summary>
	/// Restart current level.
	/// </summary>
	void Restart ();

	/// <summary>
	/// Exit the game.
	/// </summary>
	void Exit ();
}
