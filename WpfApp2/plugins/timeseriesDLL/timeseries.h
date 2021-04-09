#pragma once
/*
 * timeseries.h
 *
 *  Created on: 26 áàå÷× 2020
 *      Author: Eli
 */

#ifndef TIMESERIES_H_
#define TIMESERIES_H_
#include <iostream>
#include <string.h>
#include <fstream>
#include<map>
#include <vector>
#include <string.h>
#include <utility>
 //#include <bits/stdc++.h>
#include <algorithm>
#include <sstream>


using namespace std;

class TimeSeries {

	vector <pair<string, vector<float>>> ts;
	//map<string, vector<float>> ts;
	vector<string> atts;
	size_t dataRowSize;
public:


	TimeSeries(const char* CSVfileName) {
		ifstream in(CSVfileName);
		string head;
		in >> head;
		string att;
		stringstream hss(head);
		while (getline(hss, att, ',')) {
			pair<string, vector<float>> p;
			p.first = att;
			ts.push_back(p);
			atts.push_back(att);
		}

		while (!in.eof()) {
			string line;
			in >> line;
			string val;
			stringstream lss(line);
			int i = 0;
			while (getline(lss, val, ',')) {
				ts[i].second.push_back(stof(val));
				i++;
			}
		}
		in.close();

		dataRowSize = ts[0].second.size();

	}

	const vector<float>& getAttributeData(string name)const {
		int idx = 0;
		for (int i = 0; i < dataRowSize; i++) {
			if (ts[i].first == name) {
				idx = i;
				break;
			}
		}
		return ts[idx].second;
	}

	const vector<string>& gettAttributes()const {
		return atts;
	}

	size_t getRowSize()const {
		return dataRowSize;
	}

	~TimeSeries() {
	}
};



#endif /* TIMESERIES_H_ */