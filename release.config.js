module.exports = {
	"branches" : "master",
	"tagFormat": "v${version}",
	"plugins": [
		["@semantic-release/commit-analyzer", {
			"preset": "angular",
			"releaseRules" : [
				{"type": "docs", "release": "patch"},
				{"type": "docs", "scope": "README", "release": false}
			]
		}],
		["@semantic-release/release-notes-generator", {
			"writerOpts": {
				"commitsSort": ["scope", "subject"]
			}
		}],
		["@semantic-release/changelog", {
			"changelogFile": "CHANGELOG.md"
		}],
		["@semantic-release/npm", {
			"npmPublish": false, 
			"pkgRoot": `Assets/${process.env.PROJECT_NAME}` }],
		["@semantic-release/git", {
			"assets": [`Assets/${process.env.PROJECT_NAME}/package.json`, "CHANGELOG.md"],
			"message": "chore(release): new release ${nextRelease.version} [skip ci]\n\n${nextRelease.notes}"
		}],
		["@iam1337/create-unitypackage", {
			"packageRoot": `Assets/${process.env.PROJECT_NAME}`,
			"projectRoot": "./",
			"output": `${process.env.PROJECT_NAME}.unitypackage`
		}],
		["@semantic-release/github", {
			"assets": [
				{"path": `${process.env.PROJECT_NAME}.unitypackage`, "label": `${process.env.PROJECT_NAME} v\${nextRelease.version}`}
			]
		}]
	],
	"preset": "angular"
}
