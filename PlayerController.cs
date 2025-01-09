using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lofelt.NiceVibrations;
using UnityEngine;
using MoreMountains.Feedbacks;

public class PlayerController : MonoBehaviour {
    public GameObject _currentPiece;
    public string _currentPieceName;
    Vector2 _touchPosition;
    Vector2[] _rookDirections;
    Vector2[] _bishopDirections;
    Vector2[] _queenDirections;
    Vector2[] _pawnDirections;
    Vector2[] _knightDirections;
    [SerializeField] private GameObject _idleParticles;
    [SerializeField] private GameObject _particleGravityField;
    [SerializeField] private GameObject _knightScouter;
    [SerializeField] private ParticleSystem _slashParticle;
    private ParticleSystem _smokeParticle;
    private Ingame _iG;
    [SerializeField] private MMF_Player _effectPlayer;
    private Color32 _selectedPieceColor;
    private ParticleSystem _idlePs1, _idlePs2;
    private ParticleSystem.MainModule _module1, _module2;

    // Start is called before the first frame update
    void Start() {
        _selectedPieceColor = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Theme>().PrimaryColor;
        
        /*
        _idlePs1 = _idleParticles.transform.GetChild(0).GetComponent<ParticleSystem>();
        _idlePs2 = _idleParticles.transform.GetChild(1).GetComponent<ParticleSystem>();
        _module1 = _idlePs1.main;
        _module1.startColor = new ParticleSystem.MinMaxGradient(_selectedPieceColor);
        _module2 = _idlePs2.main;
        _module2.startColor = new ParticleSystem.MinMaxGradient(_selectedPieceColor);
*/
        _iG = GetComponent<Ingame>();

        //pawn directions
        _pawnDirections = new Vector2[]{
            new Vector2(0,1), //top mid
            new Vector2(1,0), //mid right
            new Vector2(0,-1), //bottom mid
            new Vector2(-1,0) //left mid
            };

        //rook directions
        _rookDirections = new Vector2[]{
            new Vector2(0,1), //top mid
            new Vector2(1,0), //mid right
            new Vector2(0,-1), //bottom mid
            new Vector2(-1,0) //left mid
            };

        //bishop directions
        _bishopDirections = new Vector2[]{
            new Vector2(-1, 1), //top left
            new Vector2(1,1), //top right
            new Vector2(1,-1), //bottom right
            new Vector2(-1,-1) //bottom left
            };

        //queen directions
        _queenDirections = new Vector2[]{
            new Vector2(-1, 1), //top left
            new Vector2(0,1), //top mid
            new Vector2(1,1), //top right
            new Vector2(1,0), //mid right
            new Vector2(1,-1), //bottom right
            new Vector2(0,-1), //bottom mid
            new Vector2(-1,-1), //bottom left
            new Vector2(-1,0) //left mid
            };
    }

    // Update is called once per frame
    void Update() {
        //if there's atleast one touch on the screen
        if (Input.touchCount == 1) {
            //save the first touch
            var touch = Input.GetTouch(0); //set the touch to the first one

            //if the touch just began
            if (touch.phase == TouchPhase.Began) {

                //convert the touch position
                _touchPosition = Camera.main.ScreenToWorldPoint(new Vector2(touch.position.x, touch.position.y));

                //ray cast from the touch position in no direction
                RaycastHit2D hit = Physics2D.Raycast(_touchPosition, Vector2.zero);

                //if the ray hit something
                if (hit.collider != null) {

                    var hitPiece = hit.collider.transform.gameObject; //the old piece
                    var initialCurrentPiece = _currentPiece; //the initial current piece
                    
                    //if the sprite renderer is enabled
                    if (hitPiece.GetComponent<SpriteRenderer>().enabled) {
                        hitPiece.GetComponent<SpriteRenderer>().enabled = false; //disable sprite renderer
                        
                        initialCurrentPiece.gameObject.GetComponent<SpriteRenderer>().enabled = false; //disable the sprite renderer
                        
                        //if the current piece isn't a king
                        if (!hitPiece.transform.CompareTag("king")) {
                            _iG.CheckForMasteryPiece(_currentPiece.transform.tag, false);
                            SetCurrentPiece(hit.collider.transform.parent.gameObject); //set the new current piece
                            ScanDirection(ConvertPieceNameToScanDirections(_currentPieceName)); //scan in the current piece's available directions
                            _iG.DecrementKillObjective(); //decrement the kill objective counter
                        }

                        //complete the level
                        else {
                            _iG.DisableAllHighlights(); //disable all highlights
                            _iG.HighlightKing(); //highlight the king
                            _iG.CheckForMasteryPiece(_currentPiece.transform.tag, true);
                            SetCurrentPiece(hit.collider.transform.parent.gameObject);
                            _iG.CheckForCompletion(); //check for level completion
                        }
                    }
                }
            }
        }
    }

    //this method converts theh given string to an array of directions (vector2)
    private Vector2[] ConvertPieceNameToScanDirections(string name) {
        //switch statement for name
        switch (name) {
            case "pawn":
                return _pawnDirections;
            case "rook":
                return _rookDirections;
            case "bishop":
                return _bishopDirections;
            case "queen":
                return _queenDirections;
            case "knight":
                return _knightDirections;
            default:
                return null;
        }
    }

    public void ScanDirection(Vector2[] directions) {
        _iG.DisableAllHighlights(); //disable all highlights before highlighting movable locations
        EnableCurrentPieceHighlight(); //enable the current piece highlight

        //if the piece is not a knight
        if (!_currentPiece.transform.CompareTag("knight")) {
            //cast a ray in given direction
            foreach (Vector2 direction in directions) {

                //if the piece is not a pawn
                if (!_currentPiece.transform.CompareTag("pawn")) {
                    RaycastHit2D hit = Physics2D.Raycast(_currentPiece.transform.position, direction);
                    //if the ray hits something
                    if (hit.collider != null) {
                        //show the highlight behind the piece
                        hit.collider.transform.gameObject.GetComponent<SpriteRenderer>().color = _selectedPieceColor;
                        hit.collider.transform.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                    }
                }
                //if the piece is a pawn, cast a short ray
                else {
                    RaycastHit2D hit = Physics2D.Raycast(_currentPiece.transform.position, direction, 0.5f);
                    //if the ray hits something
                    if (hit.collider != null) {
                        //show the highlight behind the piece
                        hit.collider.transform.gameObject.GetComponent<SpriteRenderer>().color = _selectedPieceColor;
                        hit.collider.transform.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                    }
                }
            }
        }

        //if the piece is a knight
        else {
            var oneUnit = _currentPiece.transform.localScale.x; //save the scale of the piece and use it as a baseline

            //the valid positions
            Vector2[] validPositions = new Vector2[] {
                //up
                new Vector2(_currentPiece.transform.localPosition.x-oneUnit, _currentPiece.transform.localPosition.y+(2*oneUnit)), //left 
                new Vector2(_currentPiece.transform.localPosition.x+oneUnit, _currentPiece.transform.localPosition.y+(2*oneUnit)), //right
                
                //right
                new Vector2(_currentPiece.transform.localPosition.x+(oneUnit*2), _currentPiece.transform.localPosition.y+oneUnit), //up
                new Vector2(_currentPiece.transform.localPosition.x+(oneUnit*2), _currentPiece.transform.localPosition.y-oneUnit), //down
                
                //down
                new Vector2(_currentPiece.transform.localPosition.x-oneUnit, _currentPiece.transform.localPosition.y-(2*oneUnit)), //left 
                new Vector2(_currentPiece.transform.localPosition.x+oneUnit, _currentPiece.transform.localPosition.y-(2*oneUnit)), //right
                
                //left
                new Vector2(_currentPiece.transform.localPosition.x-(oneUnit*2), _currentPiece.transform.localPosition.y+oneUnit), //up
                new Vector2(_currentPiece.transform.localPosition.x-(oneUnit*2), _currentPiece.transform.localPosition.y-oneUnit) //down
            };

            //if the knight scouter is not the size of the current piece, make it
            if (_knightScouter.transform.localScale.x != _currentPiece.transform.localScale.x) _knightScouter.transform.localScale = new Vector3(_currentPiece.transform.localScale.x, _currentPiece.transform.localScale.y, _currentPiece.transform.localScale.z);

            //for all the possible positions the knight can move to
            foreach (Vector2 position in validPositions) {
                //move the knight scouter to the position
                _knightScouter.transform.localPosition = position; 
                
                //cast a ray of no distance on the position
                RaycastHit2D hit = Physics2D.Raycast(_knightScouter.transform.position, Vector2.zero);
                
                //if the ray hits something
                if (hit.collider != null) {
                    //show the highlight behind the piece
                    hit.collider.transform.gameObject.GetComponent<SpriteRenderer>().color = _selectedPieceColor;
                    hit.collider.transform.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
        }
    }

    void EnableCurrentPieceHighlight() {
        var childSpriteRenderer = _currentPiece.transform.GetChild(0).GetComponent<SpriteRenderer>();
        childSpriteRenderer.color = Color.green;
        childSpriteRenderer.enabled = true;
    }
    
    //this method sets the current piece to the parameter
    public void SetCurrentPiece(GameObject piece) {
        var oldCurrentPiece = _currentPiece;
        
        //if this isn't the first call of this method
        if (_currentPiece != null){
            PlaySlashParticles(_currentPiece.transform.position); //spawn the slash particle on the old current piece location
            PlayFeedbacks(); //play the moremountains feedbacks
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact); //play vibration
            SoundManager.PlaySound("sword"); //play the slash sound
        }

        
        
        _currentPiece = piece; //set the current piece;
        _currentPieceName = _currentPiece.transform.tag; //save the tag name
        DisablePieceCollider(_currentPiece); //disable the collider to avoid rays intersecting
        
        //move the particles to the current piece
        MoveParticles();
        
        if (oldCurrentPiece!=null && _currentPiece!=null) MoveHologram(oldCurrentPiece, _currentPiece);
    }

    void MoveHologram(GameObject oldPiece, GameObject newPiece) {
        var hologram = ObjectPool.Instance.GetObject(); //get a hologram from pool
        hologram.transform.position = oldPiece.transform.position; //set the position to the old position
        hologram.GetComponent<SpriteRenderer>().sprite = oldPiece.GetComponent<SpriteRenderer>().sprite; //set the sprite of the piece
        hologram.SetActive(true); //enable the hologram
        hologram.transform.DOMove(newPiece.transform.position, 1f)
            .SetEase(Ease.OutSine)
            .OnComplete(() => { //move the hologram to the required location
            hologram.SetActive(false); //add the hologram back to queue
            });
    }

    //move the particles to the target position
    void MoveParticles() {
        _particleGravityField.transform.SetParent(null); //detatch the gravity field from the parent
        _particleGravityField.transform
            .DOLocalMove(new Vector3(_currentPiece.transform.localPosition.x, _currentPiece.transform.localPosition.y, _currentPiece.transform.localPosition.z), 0.5f);
        _idleParticles.transform.DOLocalMove(new Vector3(_currentPiece.transform.localPosition.x, _currentPiece.transform.localPosition.y, _currentPiece.transform.localPosition.z), 1f)
            .OnComplete(() => {
                _particleGravityField.transform.SetParent(_idleParticles.transform); //attatch the gravity field to the parent
                _particleGravityField.transform.localPosition = Vector2.zero; //set the local position
            });
    }

    //play the slash particles
    void PlaySlashParticles(Vector3 location) {
        //if there is no reference to the smoke particles yet, get the reference
        if (_smokeParticle == null) _smokeParticle = _slashParticle.transform.GetChild(0).GetComponent<ParticleSystem>();
        
        _slashParticle.transform.position = location; //move the slash particle to location
        _slashParticle.Play(); //play the slash
        _smokeParticle.Play(); //play the smoke
    }

    void PlayFeedbacks() {
        //if screen shake is enabled
        if (PlayerPrefs.GetInt("screenShake", 1) == 1) {
            _effectPlayer.PlayFeedbacks(); //play the shaker effect
        }
    }

    //this method disables the collider of the parameter piece
    void DisablePieceCollider(GameObject piece) {
        piece.transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = false; //disable the collider
    }
}
