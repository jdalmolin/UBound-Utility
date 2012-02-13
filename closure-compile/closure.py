#! /usr/bin/python2.4

import httplib, urllib, sys, os

#Open a file and read the contents.
f = open(sys.argv[1], 'r')

#Read the contents of the file
contents = f.readlines()

#close your files, always
f.closed

#define the parameters for the POST request and encode them in url-safe-format

params = urllib.urlencode([
    ('js_code', '\n'.join(contents)),
    ('compilation_level', 'WHITESPACE_ONLY'),
    ('output_format', 'text'),
    ('output_info','compiled_code'),
    ])

# Always use the following value for the Content-type header.
headers = { "Content-type": "application/x-www-form-urlencoded" }
conn = httplib.HTTPConnection('closure-compiler.appspot.com')
conn.request('POST', '/compile', params, headers)
response = conn.getresponse()
data = response.read()
conn.close

#do some string manip to output to a proper .min.js extension
outputfile = os.path.splitext(sys.argv[1])[0] + '.min.js'

#Open the output file, write the data, and close it.
f = open (outputfile, 'w') 
f.write(data)
    
f.closed

