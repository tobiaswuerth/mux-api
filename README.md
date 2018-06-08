**Parts:** [core](https://github.com/tobiaswuerth/mux-core) | [data](https://github.com/tobiaswuerth/mux-data) | [cli](https://github.com/tobiaswuerth/mux-cli) | [www](https://github.com/tobiaswuerth/mux-www) | *[api](https://github.com/tobiaswuerth/mux-api)*

# Mux - REST API

This is the API which provides all the essential interfaces for the [www](https://github.com/tobiaswuerth/mux-www) part to work with.

## Implementations

The following endpoints are implemented:

-----

### Login
* POST 	`public/login`					
perform login
* GET 	`auth/login`				
refresh JWT authorization token including expiration date

### Records
* GET*	`auth/records`
all records
* GET*	`auth/records/lookup/{query}`
all records where record title matches exactly query
* GET*	`auth/records/search/{query}`
all records where record title matches like query
* GET		`auth/records/{id}`
one specific record 
* GET*	`auth/records/{id}/tracks`
all tracks of one record
* GET*	`auth/records/{id}/releases`
all releases of one record
* GET*	`auth/records/{id}/artists`
all artists of one record
* GET*	`auth/records/{id}/aliases`
all aliases of one record

### Releases
* GET*	`auth/releases`
all releases
* GET*	`auth/releases/lookup/{query}`
all releases where release title matches exactly query
* GET*	`auth/releases/search/{query}`
all releases where release title matches like query
* GET		`auth/releases/{id}`
one specific release
* GET*	`auth/releases/{id}/records`
all records of a release
* GET*	`auth/releases/{id}/artists`
all artists of a release
* GET*	`auth/releases/{id}/aliases`
all aliases of a release
* GET*	`auth/releases/{id}/events`
all events of a release

### Artists
* GET*	`auth/artists`
all artists
* GET*	`auth/artists/lookup/{query}`
all artists where name matches exactly query
* GET*	`auth/artists/search/{query}`
all artists where name matches like query 
* GET		`auth/artists/{id}`
one specific artist
* GET*	`auth/artists/{id}/records`
all records of an artist
* GET*	`auth/artists/{id}/releases`
all releases of an artist

### Tracks
* GET*	`auth/tracks`
all tracks
* GET* `auth/tracks/search/{query}`
all tracks where path matches like query
* GET		`auth/tracks/{id}`
one specific track
* GET*	`auth/tracks/{id}/records`
all records of a track

### Invites
* GET* `auth/invites`
all invites of authorized user
* PUT `auth/invites`
create a new invite
* DELETE `auth/invites/{id}`
delete a specific invite
* POST `auth/invites/{token}`
use an invite to register a new user

### Files
* GET	`auth/files/{id}`
the actual file for a given track id

### Globals
* GET	`auth/globals`
some globally relevant values, e.g. total release count

### Playlists
* PUT	`auth/playlists`
creates new playlist
* PUT	`auth/playlists/{id}/entries`
creates a new entry in a playlist
* PUT	`auth/playlists/{id}/permissions`
creates or updates permission for a user in a playlist
* DELETE	`auth/playlists/{id}`
deletes a playlist
* DELETE	`auth/playlists/{playlistId}/entries/{entryId}`
deletes an entry from a playlist
* DELETE	`auth/playlists/{playlistId}/permissions/{permissionId}`
deletes a permission from a playlist
* GET*	`auth/playlists`
all playlists
* GET	`auth/playlists/{id}`
get details for one specific playlist

### Users
* GET*	`auth/users`
gets all users

## Example Bodies
All bodies are expected to be in valid JSON format.

* POST @ `auth/invites/{token}`
```json
{
    "username": "your_username",
    "password": "your_base64_password"
}
```

* POST @ `public/login`
```json
{
    "username": "your_username",
    "password": "your_base64_password"
}
```

* PUT @ `auth/playlists`
```json
{
	"name": "playlist_name"
}
```

* PUT @ `auth/playlists/{id}/entries`
```json
{
	"title": "entry_title",
	"trackId": 123
}
```

* PUT @ `auth/playlists/{id}/permissions`
```json
{
	"userId": 123,
	"canModify": false
}
```

-----

**Note:**
\* = destinations supports `?ps={int}` and `?p={int}` optional query parameters for page size and page index. If not defined, default values will be used instead.
