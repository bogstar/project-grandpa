
const functions = require('firebase-functions');
const admin = require("firebase-admin");
const serviceAccount = require("./project-grandpa-firebase-adminsdk-jjliv-a39f69aa48.json");

admin.initializeApp({
  credential: admin.credential.cert(serviceAccount),
  databaseURL: "https://project-grandpa.firebaseio.com"
});

//admin.initializeApp(functions.config().firebase);
/*admin.initializeApp({
    serviceAccountId: 'firebase-adminsdk-jjliv@project-grandpa.iam.gserviceaccount.com',
});*/

// Create and Deploy Your First Cloud Functions
// https://firebase.google.com/docs/functions/write-firebase-functions

const cors = require('cors')({origin: true});

exports.onScoreSave = functions.database
.ref('scores/data/{scoreId}')
.onCreate(async (snapshot, context) => {
    const countRef = snapshot.ref.parent.parent.child('dataCount');
    return countRef.transaction(count => {
        return count + 1;
    });
});

exports.onScoreRemove = functions.database
.ref('scores/data/{scoreId}')
.onDelete(async (snapshot, context) => {
    const countRef = snapshot.ref.parent.parent.child('dataCount');
    return countRef.transaction(count => {
        return count - 1;
    });
});

exports.authenticate = functions.https.
onRequest((request, response) => {
    let uid = request.headers['uid'];

    admin.auth().createCustomToken(uid)
    .then(function(customToken) {
        response.append('token', customToken);
        response.status(200).send();
        return null;
    })
    .catch(function(error) {
        console.log('Error creating custom token:', error);
    });
});

function authorize(successCB, request, response) {
    cors(request, response, () => {
        const tokenId = request.get('Authorization').split('Bearer ')[1];

        return admin.auth().verifyIdToken(tokenId)
        .then((decoded) => {
            successCB();
            return null;
        })
        .catch((err) => response.status(401).send(err));
    });
}

exports.saveScore = functions.https.
onRequest((request, response) => {
    authorize(() => {
        var version = request.headers['version'];
        var name = request.headers['name'];
        var score = request.headers['score'];
    
        if (typeof version === undefined || typeof name === undefined ||
            typeof name === undefined) {
            response.status(500);
        }
        else {
            if (name === "") {
                name = "Unknown";
            }
    
            admin.database().ref('version').once('value')
            .then(snapshot => {
                if (snapshot.val() === version) {
    
                    admin.database().ref('scores/data').push( { 
                        name: name,
                        score: parseInt(score),
                        date: String(new Date().toUTCString())
                    })
                    .then(snapshot => {
                        response.status(200).send();
                        return null;
                    })
                    .catch(error => {
                        console.log(error);
                        response.status(500);
                    })
                }
                else {
                    response.status(403).send("Access denied!");
                }
                return null;
            }).catch(error => {
                console.log(error);
                response.status(500);
            })
        }
    }, request, response);
});

// This HTTPS endpoint can only be accessed by your Firebase Users.
// Requests need to be authorized by providing an `Authorization` HTTP header
// with value `Bearer <Firebase ID Token>`.
//exports.app = functions.https.onRequest(app);